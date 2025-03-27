# Сервис пункта выдачи заказов (ПВЗ)

## Описание

Сервис управления заказами в пункте выдачи, включающий следующие функции:
- Принятие заказа от курьера
- Выдача заказа клиенту
- Приём возврата от клиента
- Возврат заказа курьеру
- Хранение в кэше списка актуальных заказов
- Просмотр и хранение в кэше истории заказов

## Архитектура сервиса

#### Принятие заказа от курьера

```mermaid
sequenceDiagram
    participant Client as Клиент API
    participant Service as Сервис заказов
    participant TxManager as Транзакционный менеджер
    participant Repo as Репозиторий заказов
    participant DB as База данных
    participant Cache as Кэш активных заказов

    Client->>Service: AcceptOrder(orderData)
    Service->>TxManager: RunInTransaction
    TxManager->>Repo: GetByID(orderID)
    Repo->>DB: SELECT * FROM orders WHERE id=?
    DB-->>Repo: order (или null)
    Repo-->>TxManager: order (или null)
    
    alt Проверка существования
        TxManager-->>Service: order exists
        Service-->>Client: Ошибка: заказ уже существует
    else Заказ не существует
        Service->>TxManager: RunInTransaction
        TxManager->>Repo: Create(order)
        Repo->>DB: INSERT INTO orders
        DB-->>Repo: OK
        Repo-->>TxManager: OK
        TxManager-->>Service: OK
        Service->>Cache: InvalidateActiveOrders(userID)
        Service-->>Client: Успех: заказ создан
    end
```

#### Выдача заказа клиенту

```mermaid
sequenceDiagram
    participant Client as Клиент API
    participant Service as Сервис заказов
    participant TxManager as Транзакционный менеджер
    participant Repo as Репозиторий заказов
    participant DB as База данных
    participant Cache as Кэш активных заказов

    Client->>Service: DeliverOrdersToUser(userID, orderIDs)
    
    loop для каждого orderID
        Service->>TxManager: RunInTransaction
        TxManager->>Repo: GetByID(orderID)
        Repo->>DB: SELECT * FROM orders WHERE id=?
        DB-->>Repo: order
        Repo-->>TxManager: order
        
        alt Проверки валидности
            TxManager-->>Service: Ошибка: заказ не найден/не принадлежит пользователю/не доступен
            Service-->>Client: Ошибка выдачи заказа
        else Заказ валиден
            TxManager->>Repo: Update(order с статусом "DeliveredToUser")
            Repo->>DB: UPDATE orders SET status="delivered_to_user", delivered_at=NOW()
            DB-->>Repo: OK
            Repo-->>TxManager: OK
            TxManager-->>Service: OK
        end
    end
    
    Service->>Cache: InvalidateActiveOrders(userID)
    Service-->>Client: Успех: заказы выданы
```

#### Приём возврата от клиента

```mermaid
sequenceDiagram
    participant Client as Клиент API
    participant Service as Сервис заказов
    participant TxManager as Транзакционный менеджер
    participant Repo as Репозиторий заказов
    participant DB as База данных
    participant Cache as Кэш активных заказов

    Client->>Service: AcceptReturnsFromUser(userID, orderIDs)
    
    loop для каждого orderID
        Service->>TxManager: RunInTransaction
        TxManager->>Repo: GetByID(orderID)
        Repo->>DB: SELECT * FROM orders WHERE id=?
        DB-->>Repo: order
        Repo-->>TxManager: order
        
        alt Проверки валидности
            TxManager-->>Service: Ошибка: заказ не найден/не принадлежит пользователю/не выдан
            Service-->>Client: Ошибка приёма возврата
        else Заказ валиден для возврата
            TxManager->>Repo: Update(order с статусом "ReturnedByUser")
            Repo->>DB: UPDATE orders SET status="returned_by_user", returned_at=NOW()
            DB-->>Repo: OK
            Repo-->>TxManager: OK
            TxManager-->>Service: OK
        end
    end
    
    Service->>Cache: InvalidateActiveOrders(userID)
    Service-->>Client: Успех: возврат принят
```

#### Возврат заказа курьеру

```mermaid
sequenceDiagram
    participant Client as Клиент API
    participant Service as Сервис заказов
    participant TxManager as Транзакционный менеджер
    participant Repo as Репозиторий заказов
    participant DB as База данных
    participant Cache as Кэш активных заказов

    Client->>Service: ReturnOrderToCourier(orderID)
    Service->>TxManager: RunInTransaction
    TxManager->>Repo: GetByID(orderID)
    Repo->>DB: SELECT * FROM orders WHERE id=?
    DB-->>Repo: order
    Repo-->>TxManager: order
    
    alt Проверки валидности
        TxManager-->>Service: Ошибка: заказ не найден/не возвращён/срок хранения
        Service-->>Client: Ошибка возврата курьеру
    else Заказ валиден для возврата курьеру
        TxManager->>Repo: Delete(orderID)
        Repo->>DB: DELETE FROM orders WHERE id=?
        DB-->>Repo: OK
        Repo-->>TxManager: OK
        TxManager-->>Service: OK, userID заказа
        Service->>Cache: InvalidateActiveOrders(userID)
        Service-->>Client: Успех: заказ возвращён курьеру
    end
```

### Механизм кэширования

```mermaid
graph TD
    A[Запуск сервиса] --> B[Инициализация кэша]
    B --> C[Прогрев кэша активных заказов]
    
    D[Запрос активных заказов] --> E{Есть в кэше?}
    E -->|Да| F[Вернуть из кэша]
    E -->|Нет| G[Запрос из БД]
    G --> H[Сохранить в кэш]
    H --> F
    
    I[Изменение статуса заказа] --> J[Инвалидация кэша для пользователя]
    
    K[Фоновый обновитель] --> L[Периодическое обновление кэша истории]
    L --> M[Для каждого активного пользователя]
    M --> N[Обновить историю в кэше]
    
    O[Запрос истории заказов] --> P{Фильтрация?}
    P -->|Да| Q[Прямой запрос в БД]
    P -->|Нет| R{Есть в кэше?}
    R -->|Да| S[Применить курсор/лимит]
    R -->|Нет| T[Запрос из БД]
    T --> U[Сохранить в кэш]
    U --> S
```

## Особенности реализации

1. **Транзакционная безопасность**: Все операции с данными в БД выполняются в транзакциях, что гарантирует целостность данных и атомарность операций.

2. **Кэширование**:
   - Активные заказы кэшируются для быстрого доступа
   - История заказов кэшируется и обновляется в фоновом режиме
   - Кэш инвалидируется при изменении статуса заказа

3. **Проверки**:
   - Проверка принадлежности заказа пользователю
   - Проверка валидности состояния заказа для операции
   - Проверка сроков хранения и возврата

4. **Мемкэш**: Используется Memcached для хранения кэша, что обеспечивает:
   - Быстрый доступ к данным
   - Автоматическое истечение срока жизни (TTL)
   - Распределенное хранение

## Запуск сервиса

```bash
make build
./bin/pvz-service
```

Конфигурация сервиса находится в файле `configs/config.yml`.

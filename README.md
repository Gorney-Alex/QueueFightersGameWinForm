# Что у меня сейчас имеется

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





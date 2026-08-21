namespace RestaurantManagement.API.Entities;
public enum UserRole { Admin, Manager, Cashier, Chef, Waiter }
public enum TableStatus { Available, Occupied, Reserved }
public enum OrderStatus { Pending, Cooking, Ready, Served, Paid, Cancelled }
public enum PaymentMethod { Cash, QrCode, CreditCard }
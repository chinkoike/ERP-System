namespace ERP.Sales.Domain;

public enum OrderStatus
{
    Pending = 0,    // รอชำระเงิน/รอยืนยัน
    Confirmed = 1,  // ยืนยันคำสั่งซื้อแล้ว
    Processing = 2, // กำลังจัดเตรียมสินค้า
    Shipped = 3,    // ส่งสินค้าแล้ว
    Delivered = 4,  // สินค้าถึงมือลูกค้าแล้ว
    Cancelled = 5   // ยกเลิกคำสั่งซื้อ
}
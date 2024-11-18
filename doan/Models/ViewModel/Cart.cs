using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace doan.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        //them san pham vao gio
        public void AddItem(int productId, string productImage, string productName, decimal unitPrice, int quantity, string category)
        {
            var exisitingItem = items.FirstOrDefault(i => i.ProductID == productId);
            if (exisitingItem == null)
            {
                items.Add(new CartItem { ProductID = productId, ProductImage = productImage, ProductName = productName, UnitPrice = unitPrice, Quantity = quantity });

            }
            else
            {
                exisitingItem.Quantity += quantity;
            }
        }
        //xoa san pham khoi gio hang
        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        //tinh tong gia tri gio hang
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        //lam trong gio hang
        public void Clear()
        {
            items.Clear();
        }

        //cap nhat so luong cua san pham da chon
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}
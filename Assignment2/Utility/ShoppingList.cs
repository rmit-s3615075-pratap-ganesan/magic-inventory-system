using System;
using System.Collections.Generic;
using Assignment2.Models.CartViewModels;
using Microsoft.AspNetCore.Http;

namespace Assignment2.Utility
{
    public static class ShoppingList
    {
        public static Dictionary<String, CartViewModel> cartList = new Dictionary<string, CartViewModel>();

        public static Dictionary<String, CartViewModel> GetAllShoppingList()
        {
            return cartList;
        }

        public static void AddToShoppingList(CartViewModel cartValue)
        {

            string cartkey = cartValue.ProductID + "/" + cartValue.StoreID;
            if (cartList.ContainsKey(cartkey))
                DeleteSessionKeys(cartkey);
            cartList.Add(cartkey, cartValue);
        }

        public static void DeleteSessionKeys(String cartKey)
        {
            cartList.Remove(cartKey);
        }

        public static void AddToShoppingListSession(CartViewModel cartValue)
        {

            string cartkey = cartValue.ProductID + "/" + cartValue.StoreID;
         }
    }
}
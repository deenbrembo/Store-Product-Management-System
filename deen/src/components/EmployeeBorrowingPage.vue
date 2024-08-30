<template>
    <div>
      <div class="search-bar">
        <input v-model="searchQuery" type="text" placeholder="Search products by productName" />
        <button @click="toggleQRScanner">
          <i class="fas fa-qrcode"></i> Scan QR Code
        </button>
        <button @click="toggleCart">
          <i class="fas fa-shopping-cart"></i> Cart ({{ cart.length }})
        </button>
      </div>
      <div class="product-list">
        <div v-for="product in filteredproducts" :key="product.id" class="product-card">
          <img :src="product.imageUrl" alt="product image" class="product-image" />
          <h3>{{ product.productName }}</h3>
          <p>{{ product.description }}</p>
          <p>quantityAvalaible: {{ product.quantityAvalaible }}</p>
          <p>status: {{ product.status ? 'Available' : 'Not Available' }}</p>
          <button @click="openBorrowPopup(product)">Borrow</button>
        </div>
      </div>
  
      <!-- QR Code Scanner -->
      <QRCodeScanner v-if="isScannerVisible" @close="isScannerVisible = false" @scan="handleScan" />
  
      <!-- Borrow Popup Container with Transition -->
      <transition productName="fade">
        <div v-if="isPopupVisible" class="popup-container">
          <div class="popup-overlay">
            <button class="close-popup" @click="closeBorrowPopup">
              <i class="fas fa-times"></i>x</button>
            <h2>Borrow product</h2>
            <img :src="selectedproduct.imageUrl" alt="Selected product image" class="popup-image" />
            <p><strong>product:</strong> {{ selectedproduct.productName }}</p>
            <p><strong>description:</strong> {{ selectedproduct.description }}</p>
            <p><strong>quantityAvalaible:</strong> {{ selectedproduct.quantityAvalaible }}</p>
            <p><strong>status:</strong> {{ selectedproduct.status ? 'Available' : 'Not Available' }}</p>
            <div class="form-group">
              <label for="quantity">Quantity:</label>
              <input id="quantity" v-model="borrowQuantity" type="number" min="1" :max="selectedproduct.quantityAvalaible" />
            </div>
            <button @click="borrowproduct">Borrow</button>
            <button @click="addToCart">Add to Cart</button>
          </div>
        </div>
      </transition>
  
      <!-- Cart Popup Container with Transition -->
      <transition productName="fade">
        <div v-if="isCartVisible" class="cart-popup-container">
          <div class="cart-overlay">
        <button class="close-cart" @click="closeCart">
          <i class="fas fa-times"></i>x</button>
        <h2>Your Cart</h2>
        <div v-for="(product, index) in cart" :key="product.id" class="cart-product">
          <img :src="product.imageUrl" alt="Cart product image" class="cart-product-image" />
          <p>{{ product.productName }}</p>
          <div class="cart-quantity">
              <button @click="decreaseQuantity(index)" class="quantity-button-add">-</button>
              <input type="text" v-model="product.quantity" min="1"/>
              <button @click="increaseQuantity(index)" class="quantity-button-minus">+</button>
          </div>
          <button @click="removeFromCart(index)" class="remove-product">Remove</button>
        </div>
        <button @click="checkout" class="checkout-button">Checkout</button>
        <p v-if="cart.length === 0">Your cart is empty.</p>
          </div>
        </div>
      </transition>
        </div>
                <!-- Notification Popup -->
                <transition productName="slide-fade">
        <div v-if="notification.visible" :class="['notification', notification.type, { show: notification.visible }]">
          <p>{{ notification.message }}</p>
          <button @click="closeNotification">✕</button>
        </div>
      </transition>
      </template>
  
  <script>
  import QRCodeScanner from './QRCodeScanner.vue';
  
  const config = {
  apiBaseUrl: window.location.hostname === 'localhost' 
    ? 'http://localhost:3000/api' 
    : 'http://192.168.1.14:3000/api'
};
  export default {
    components: {
      QRCodeScanner
    },
    data() {
      return {
        products: [], // List of products
        searchQuery: '',
        isScannerVisible: false,
        isPopupVisible: false,
        isCartVisible: false,
        selectedproduct: null,
        borrowQuantity: 1,
        cart: this.loadCartFromLocalStorage(), // Initialize cart from local storage
        notification: {
          visible: false,
          type: '', // 'success' or 'error'
          message: ''
        }
      };
    },
    computed: {
      filteredproducts() {
        return this.products.filter(product =>
          product.productName.toLowerCase().includes(this.searchQuery.toLowerCase()) && product.status !== 'Removed'
        );
      }
    },
    methods: {
  async fetchProducts() {
    try {
      const response = await fetch(`${config.apiBaseUrl}/Auth/products`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      this.products = await response.json();
    } catch (error) {
      console.error('Failed to fetch products:', error);
    }
  },
async borrowproduct() {
    try {
        const response = await fetch(`${config.apiBaseUrl}/Auth/employee-borrow-product`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({
                ID: this.selectedproduct.id,
                Quantity: this.borrowQuantity
            })
        });
        const data = await response.json();
        if (response.ok) {
            this.showNotification('success', data.message);
            this.closeBorrowPopup();
            this.fetchProducts(); // Refresh products after borrowing
        } else {
            this.showNotification('error', data.message);
        }
    } catch (error) {
        console.error('Failed to borrow product:', error);
        this.showNotification('error', 'Failed to borrow product.');
    }
},

  saveCartToLocalStorage() {
    localStorage.setItem('cart', JSON.stringify(this.cart)); // Use setItem
  },
  loadCartFromLocalStorage() {
    const cartData = localStorage.getItem('cart'); // Use getItem
    return cartData ? JSON.parse(cartData) : [];
  },
  toggleQRScanner() {
    this.isScannerVisible = !this.isScannerVisible;
  },
  handleScan(result) {
    console.log('QR Code scanned:', result);
    this.isScannerVisible = false;

    // Assuming result contains product ID or a way to find the product
    const scannedproduct = this.products.find(product => product.Id === result);
    if (scannedproduct) {
      this.openBorrowPopup(scannedproduct);
    } else {
      console.warn('Scanned product not found:', result);
    }
  },
  openBorrowPopup(product) {
    this.selectedproduct = product;
    this.borrowQuantity = 1; // Reset quantity
    this.isPopupVisible = true;
  },
  closeBorrowPopup() {
    this.isPopupVisible = false;
    this.selectedproduct = null;
  },
  addToCart() {
    const productInCart = this.cart.find(cartproduct => cartproduct.productName === this.selectedproduct.productName);

    if (productInCart) {
      // If product is already in the cart, update the quantity
      productInCart.quantity += this.borrowQuantity;
    } else {
      // If product is not in the cart, add it as a new entry
      this.cart.push({
        ...this.selectedproduct,
        quantity: this.borrowQuantity
      });
    }

    // Save cart to local storage and close the popup after adding to cart
    this.saveCartToLocalStorage();
    this.closeBorrowPopup();
  },
  toggleCart() {
    this.isCartVisible = !this.isCartVisible;
  },
  closeCart() {
    this.isCartVisible = false;
  },
  decreaseQuantity(index) {
    if (this.cart[index].quantity > 1) {
      this.cart[index].quantity--;
      this.saveCartToLocalStorage(); // Update local storage
    }
  },
  increaseQuantity(index) {
    this.cart[index].quantity++;
    this.saveCartToLocalStorage(); // Update local storage
  },
  removeFromCart(index) {
    this.cart.splice(index, 1);
    this.saveCartToLocalStorage(); // Update local storage
  },
  async checkout() {
    if (confirm('Do you confirm you want to borrow all these products?')) {
      try {
        const response = await fetch(`${config.apiBaseUrl}/Auth/employee-borrow-many-products`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${localStorage.getItem('authToken')}`
          },
          body: JSON.stringify({
            Products: this.cart.map(product => ({
              ID: product.id,
              Quantity: product.quantity
            }))
          })
        });
        const data = await response.json();
        if (response.ok) {
          this.showNotification('success', data.message);
          this.cart = []; // Clear cart after checkout
          this.saveCartToLocalStorage(); // Update local storage
          this.fetchProducts(); // Refresh products after borrowing
        } else {
          this.showNotification('error', data.message);
        }
      } catch (error) {
        this.showNotification('error', 'Failed to borrow products. Please try again.');
      }
      this.closeCart();
    }
  },
  showNotification(type, message) {
    this.notification.type = type;
    this.notification.message = message;
    this.notification.visible = true;
    
    // Automatically hide the notification after 5 seconds
    setTimeout(() => {
      this.closeNotification();
    }, 5000);
  },
  closeNotification() {
    this.notification.visible = false;
  }
},
mounted() {
  this.fetchProducts();
}
}
</script>
  
  <style scoped>
  /* Existing styles remain unchanged */
  
  .search-bar {
      display: flex;
      align-items: center;
      margin-bottom: 50px;
      margin-top: 450px;
      }
      
      input {
      flex: 1;
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 5px;
      
      }
      
      button {
      margin-left: 10px;
      }
      
      .product-list {
      display: flex;
      flex-wrap: wrap;
      gap: 44px; /* Space between products */
      }
      
      .product-card {
      border: 1px solid #c1d8e9;
      padding: 20px;
      border-radius: 10px;
      width: calc(20% - 20px); /* 5 columns with space between */
      text-align: center;
      background-color: rgba(0, 140, 191, 0.303);
      transition: transform 0.3s ease, box-shadow 0.3s ease;
      }
      
      .product-card:hover {
      transform: translateY(-10px); /* Float effect on hover */
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
      }
      
      .product-image {
      max-width: 200px;
      width: 100%;
      height:100%;
      max-height: 150px;
      object-fit: cover;
      border-radius: 5px;
      }
      
      .popup-container {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
      animation: fadeIn 0.5s ease-in-out;
      }
      
      .popup-overlay {
      position: relative;
      width: 80%;
      max-width: 400px;
      background: linear-gradient(to right, #d5fff6, #b7d4f0);
      border-radius: 10px;
      padding: 20px;
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
      transition: transform 0.3s ease-in-out;
      animation: scaleIn 0.5s ease-out;
      }
      
      .close-popup {
        position: absolute;
        top: 10px;
        right: 10px;
        background: linear-gradient(to right, #ff4d4d, #ff9e4d);
        color: rgb(234, 213, 213);
        border: none;
        border-radius: 50%;
        padding: 5px 7px;
        cursor: pointer;
        font-size: 14px;
      }
  
      .close-popup:hover {
        background: linear-gradient(to right, #e0aa3e, #ff4d4d);
      }
      
      .close-popup i {
      margin: 0;
      }
      
      .cart-popup-container {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 1000;
        animation: fadeIn 0.5s ease-in-out;
      }
      
      .cart-overlay {
        position: relative;
        width: 90%;
        max-width: 600px;
        background: linear-gradient(to right, #d5fff6, #b7d4f0);
        border-radius: 10px;
        padding: 20px;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        transition: transform 0.3s ease-in-out;
        animation: scaleIn 0.5s ease-out;
        overflow-y: auto; /* Allow scrolling if content overflows */
      }
      
      .close-cart {
        position: absolute;
        top: 10px;
        right: 10px;
        background: linear-gradient(to right, #ff4d4d, #ff9e4d);
        color: rgb(234, 213, 213);
        border: none;
        border-radius: 50%;
        padding: 5px 7px;
        cursor: pointer;
        font-size: 14px;
      }
      
      .close-cart:hover {
        background: linear-gradient(to right, #ff9e4d, #ff4d4d);
      }
      
      .cart-product {
        display: grid;
        grid-template-columns: 80px 1fr 120px 80px; /* Columns for image, productName, quantity, and action */
        align-items: center;
        border-bottom: 1px solid #ddd;
        padding: 10px 0;
      }
      
      .cart-product-image {
        width: 60px;
        height: 60px;
        object-fit: cover;
        border-radius: 5px;
      }
      
      .cart-product-info {
        display: flex;
        flex-direction: column;
      }
      
      .cart-product-productName {
        font-weight: bold;
        margin-bottom: 5px;
      }
      
      .cart-quantity {
        display: flex;
        align-items: center;
        gap: 10px;
      }
      
      .cart-quantity button {
        background: linear-gradient(to right, #6bc0f6, #1f61d2);
        border: 1px solid #ccc;
        border-radius: 4px;
        padding: 5px;
        cursor: pointer;
      }
      
      .quantity-button-add {
        margin-right: 5px;
        border-top-left-radius: 4px;
        border-bottom-left-radius: 4px;
      }
      
      .quantity-button-minus {
        margin-left: 5px;
        border-top-right-radius: 4px;
        border-bottom-right-radius: 4px;
      }
      
      .cart-quantity input {
        width: 10px;
        height: 13px;
        text-align: center;
        border: 1px solid #ccc;
        border-radius: 4px;
      }
      
      .remove-product {
        background: #e03e3e;
        width: 100%;
        color: white;
        border: none;
        border-radius: 4px;
        padding: 5px 10px;
        cursor: pointer;
      }
      
      .remove-product:hover {
        background: linear-gradient(to right, #e0aa3e, #ff4d4d);
      }
      
      .checkout-button {
        border-color: #61bd4a;
        color: rgb(49, 160, 49);
      }
      
      .checkout-button:hover {  
        background: linear-gradient(to right, #7bf66b, #1fd26d);
        color: white;
      }
      
      
      /* Transition Styles */
      .fade-leave-active {
      transition: opacity 0.7s ease;
      }
      
      
      .fade-enter, .fade-leave-to {
      opacity: 0;
      }
      
      /* Popup Content Animation*/
      @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
      }
      
      @keyframes scaleIn {
      from {
        transform: scale(0.8);
      }
      to {
        transform: scale(1);
      }
      }
      
      @keyframes scaleOut {
      from {
        transform: scale(1);
      }
      to {
        transform: scale(0.8);
      }
      }
      
      
      
      button {
    border: 2px solid black;
    border-color: #00a1f1;
    border-radius: 10px;
    color: rgb(0, 150, 209);
    padding: 5px 24px;
    font-size: 16px;
    cursor: pointer;
      }
      
      button:hover {
        background: linear-gradient(to right, #6bc0f6, #1f61d2);
        color: white;
      }
      
      .form-group {
      margin-bottom: 15px;
      }
      
      label {
      display: block;
      margin-bottom: 5px;
      }
      
      input[type="number"] {
      width: 100%;
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      }
      
      .popup-image {
      width: 200px;
      height: 200px; /* Set a fixed height for the popup image */
      object-fit: cover; /* Make sure the image covers the container */
      border-radius: 5px;
      margin-bottom: 15px;
      }
      
      
      
      /* Mobile Styles */
      @media (max-width: 768px) {
      .product-list {
        gap: 20px; /* Reduce gap between products */
        flex-direction: column; /* Stack products vertically */
        max-height: none; /* Allow full height for scrolling */
      }
      
      .product-card {
        width: 100%; /* Full width on mobile */
        text-align: left; /* Align text to left */
      }
      
      .product-image {
        width: 140px;
        height: 80px;
        object-fit: cover;
        border-radius: 5px;
      }
      
      .popup-overlay {
        width: 90%; /* Adjust popup width for mobile */
        max-width: none; /* Remove max-width restriction */
      }
      
      .popup-image {
        width: 140px;
        height: 80px;
        object-fit: cover;
        border-radius: 5px;
      }
      input[type="number"] {
        width: 90%; /* Decrease input width on mobile */
      }
      .cart-overlay {
        width: 88%; /* Adjust width for mobile */
        max-width: none; /* Remove max-width restriction */
      
      }
  
    button {
    border: 2px solid black;
    border-color: #0473aa;
    border-radius: 10px;
    color: rgb(0, 92, 128);
    padding: 7px 13px;
    font-size: 16px;
    cursor: pointer;
      }
      
      button:hover {
        background: linear-gradient(to right, #6bc0f6, #1f61d2);
        color: white;
      }
  
      .quantity-button-add {
        margin-right: 1px;
        border-top-left-radius: 4px;
        border-bottom-left-radius: 4px;
      }
      
      .quantity-button-minus {
        margin-left: 1px;
        border-top-right-radius: 4px;
        border-bottom-right-radius: 4px;
      }
      
      .cart-quantity input {
        width: 5px;
        height: 13px;
        text-align: center;
        border: 1px solid #ccc;
        border-radius: 4px;
      }
      
      .remove-product {
        background: #e03e3e;
        width: 100%;
        color: white;
        border: none;
        border-radius: 4px;
        padding: 5px 10px;
        cursor: pointer;
      }
      
      .remove-product:hover {
        background: linear-gradient(to right, #e0aa3e, #ff4d4d);
      }
  
      }
      
      @media (max-width: 480px) {
      .product-card {
        width: 95%;
        padding: 10px; /* Reduce padding in product cards */
      }
      
      .product-image {
        height: 120px; /* Further reduce image height for very small screens */
      }
      
      .popup-overlay {
        padding: 10px; /* Reduce padding in popup overlay */
      }
      
      .popup-image {
        height: 130px; /* Further adjust height for very small screens */
      }
      
      .cart-overlay {
        width: 85%; /* Adjust width for mobile */
        max-width: none; /* Remove max-width restriction */
      
      }
      }
      
      
        /* Notification Styles */
        .notification {
        position: fixed;
        top: 20px;
        right: -300px; /* Start off-screen */
        max-width: 300px;
        padding: 10px; /* Adjusted padding for smaller gap */
        border-radius: 8px;
        color: #fff;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
        z-index: 1000;
        display: flex;
        justify-content: space-between;
        align-items: center;
        background-color: #333;
      }
      
      .notification.success {
        background-color: #4caf50; /* Green for success */
      }
      
      .notification.error {
        background-color: #f44336; /* Red for error */
      }
      
      .notification.show {
        right: 20px; /* Slide to visible position */
      }
      
      .notification button {
        background: none;
        border: none;
        color: #fff;
        font-size: 16px;
        cursor: pointer;
        margin-left: 10px; /* Adjusted margin for smaller gap */
      }
      
      .notification button:hover {
        color: #ddd;
      }
      
      .notification p {
        margin: 0;
        padding-right: 10px;
      }
      
      /* Keyframe animations */
      @keyframes slide-in {
        0% {
          right: -300px; /* Start off-screen */
        }
        100% {
          right: 20px; /* End at visible position */
        }
      }
      
      @keyframes slide-out {
        0% {
          right: 20px; /* Start at visible position */
        }
        100% {
          right: -300px; /* End off-screen */
        }
      }
      
      .slide-fade-enter-active {
        animation: slide-in 0.3s ease forwards;
      }
      
      .slide-fade-leave-active {
        animation: slide-out 0.3s ease forwards;
      }
  
  </style>
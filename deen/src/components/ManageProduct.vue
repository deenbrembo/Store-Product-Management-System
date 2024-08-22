<template>
    <div class="manage-item">
    <button @click="showRegisterPopup = true; clearForm()" class="btn-register">Register New Product</button>
  
      <div class="search-container">
        <input v-model="searchQuery" type="text" placeholder="Search products..." />
        <select v-model="statusFilter" class="status-filter">
        <option value="">Filter by Status</option>
        <option value="Available">Available</option>
        <option value="Unavailable">Out of Stock</option>
      </select>
      </div>
  
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th @click="sortFiltered('productName')">Name</th>
              <th @click="sortFiltered('description')">description</th>
              <th @click="sortFiltered('quantityAvalaible')">Stock</th>
              <th @click="sortFiltered('status')">Availability</th>
              <th>Product Image</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in filteredProducts" :key="product.ID">
              <td>{{ product.productName }}</td>
              <td>{{ product.description }}</td>
              <td>{{ product.quantityAvalaible }}</td>
              <td>{{ product.status }}</td>
              <td>
                <a :href="product.imageUrl" target="_blank">
                  <img :src="product.imageUrl" alt="Product Image" />
                </a>
              </td>
              <td>
                <button v-if="product.status === 'Available'" @click="editProduct(product)" class="btn-edit">Edit</button>
                <button v-if="product.status === 'Available'" @click="confirmDelete(product)" class="btn-delete">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <!-- Register Product Popup -->
      <div v-if="showRegisterPopup" class="popup-container">
        <div class="popup-content">
          <h3>Register New Product</h3>
          <form @submit.prevent="registerProduct">
            <label for="productName">Name:</label>
            <input v-model="newProduct.productName" id="productName" required />
  
            <label for="productdescription">description:</label>
            <input v-model="newProduct.description" id="productdescription" required />
  
            <label for="productquantity">Stock:</label>
            <input v-model.number="newProduct.quantityAvalaible" id="productquantity" type="number" min="0" required />

            <label for="productimageUrl">Image URL:</label>
            <input type="file" @change="handleImageUpload" accept="image/*" />
  
            <button type="submit" class="btn-register2">Register</button>
            <button type="button" class="btn-cancel" @click="showRegisterPopup = false">Cancel</button>
          </form>
        </div>
      </div>
  
      <!-- Edit Product Popup -->
      <div v-if="showEditPopup" class="popup-container">
        <div class="popup-content">
          <h3>Edit Product</h3>
          <form @submit.prevent="updateProduct">
            <label for="editproductName">Name:</label>
            <input v-model="currentProduct.productName" id="editproductName" required />
  
            <label for="editProductdescription">description:</label>
            <input v-model="currentProduct.description" id="editProductdescription" required />
  
            <label for="editProductquantity">Stock:</label>
            <input v-model.number="currentProduct.quantityAvalaible" id="editProductquantity" type="number" min="0" required />
  
            <label for="editProductimageUrl">Image URL:</label>
            <input type="file" @change="handleImageUpdateUpload" accept="image/*">

            
            <button type="submit" class="btn-edit2">Update</button>
            <button type="button" class="btn-cancel" @click="showEditPopup = false;">Cancel</button>
          </form>
        </div>
      </div>
  
      <!-- Notification Popup -->
      <transition name="slide-fade">
        <div v-if="notification.visible" :class="['notification', notification.type, { show: notification.visible }]">
          <p>{{ notification.message }}</p>
          <button @click="closeNotification">✕</button>
        </div>
      </transition>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        products: [],
        searchQuery: '',
        statusFilter: '',
        showRegisterPopup: false,
        showEditPopup: false,
        newProduct: {
          productName: '',
          description: '',
          quantityAvalaible: 0,
          imageUrl: ''
        },
        currentProduct: null,
        notification: {
          visible: false,
          type: '',
          message: ''
        },
        sortKey: '',
        sortDirection: 'asc'
      };
    },
    computed: {
      filteredProducts() {
        let filteredProducts = this.products.filter(product =>
          product.productName.toLowerCase().includes(this.searchQuery.toLowerCase()) && product.status !== 'Removed'
        );

              // Filter by status
      if (this.statusFilter) {
        filteredProducts = filteredProducts.filter(employee => employee.status === this.statusFilter);
      }
  
        if (this.sortKey) {
          filteredProducts.sort((a, b) => {
            const valueA = a[this.sortKey];
            const valueB = b[this.sortKey];
  
            if (valueA < valueB) {
              return this.sortDirection === 'asc' ? -1 : 1;
            }
            if (valueA > valueB) {
              return this.sortDirection === 'asc' ? 1 : -1;
            }
            return 0;
          });
        }
  
        return filteredProducts;
      }
    },
    methods: {
      async fetchProducts() {
        try {
          const response = await fetch('http://localhost:5041/api/Auth/products', {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('authToken')}`
            }
          });
          this.products = await response.json();
        } catch (error) {
          console.error('Failed to fetch products:', error);
        }
      },
      async registerProduct() {
        try {
          await fetch('http://localhost:5041/api/Auth/add-product', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify(this.newProduct)
          });
          this.showRegisterPopup = false;
          this.showNotification('success', 'Product registered successfully!');
          this.fetchProducts();
        } catch (error) {
          console.error('Failed to register product:', error);
          this.showNotification('error', 'Failed to register product. Please try again.');
        }
      },
      editProduct(product) {
        this.currentProduct = { ...product };
        this.showEditPopup = true;
      },
      async updateProduct() {
        try {
          await fetch('http://localhost:5041/api/Auth/update-product', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify(this.currentProduct)
          });
          this.showEditPopup = false;
          this.showNotification('success', 'Product updated successfully!');
          this.fetchProducts();
        } catch (error) {
          console.error('Failed to update product:', error);
          this.showNotification('error', 'Failed to update product. Please try again.');
        }
      },
      async confirmDelete(product) {
        if (confirm('Are you sure you want to delete this product?')) {
          try {
            await fetch('http://localhost:5041/api/Auth/remove-product', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${localStorage.getItem('authToken')}`
              },
              body: JSON.stringify({ ID: product.id })
            });
            this.showNotification('success', 'Product deleted successfully!');
            this.fetchProducts();
          } catch (error) {
            console.error('Failed to delete product:', error);
            this.showNotification('error', 'Failed to delete product. Please try again.');
          }
        }
      },
      sortFiltered(key) {
        this.sortKey = key;
        this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
      },
      showNotification(type, message) {
        this.notification.type = type;
        this.notification.message = message;
        this.notification.visible = true;
        setTimeout(this.closeNotification, 3000);
      },
      closeNotification() {
        this.notification.visible = false;
      },
        clearForm() {
            this.newProduct.productName = '';
            this.newProduct.description = '';
            this.newProduct.quantityAvalaible = 0;
            this.newProduct.imageUrl = '';
        },
        handleImageUpload(event) {
            const file = event.target.files[0];
            const reader = new FileReader();
            reader.onload = () => {
                this.newProduct.imageUrl = reader.result;
            };
            reader.readAsDataURL(file);
        },
        handleImageUpdateUpload(event) {
            const file = event.target.files[0];
            const reader = new FileReader();
            reader.onload = () => {
                this.currentProduct.imageUrl = reader.result;
            };
            reader.readAsDataURL(file);
        }
    },
    created() {
      this.fetchProducts();
    }
  };
  </script>
    
    <style scoped>

td a {
        display: block;
        width: 100%;
        height: 100%;
    }

    td img {
        width: 100%;
        max-width: 100px;
        height: auto;
    }

    body {
      background: #fafafa url(https://jackrugile.com/images/misc/noise-diagonal.png);
      color: #444;
      font: 100%/30px 'Helvetica Neue', helvetica, arial, sans-serif;
      text-shadow: 0 1px 0 #fff;
    }
    
    strong {
      font-weight: bold; 
    }
    
    em {
      font-style: italic; 
    }
    
    .table-container {
      max-width: 900px;
      margin: 30px auto;
      overflow-y: auto;
      max-height: 400px; /* Set fixed height for table container */
    }
    
    table {
      background: #f5f5f5;
      border-collapse: separate;
      box-shadow: inset 0 1px 0 #fff;
      font-size: 12px;
      line-height: 24px;
      width: 100%;
      min-width: 800px;
    }  
    
    th {
      background: url(https://jackrugile.com/images/misc/noise-diagonal.png), linear-gradient(#777, #444);
      border-left: 1px solid #555;
      border-right: 1px solid #777;
      border-top: 1px solid #555;
      border-bottom: 1px solid #333;
      box-shadow: inset 0 1px 0 #999;
      color: #fff;
      font-weight: bold;
      padding: 10px 15px;
      position: relative;
      text-shadow: 0 1px 0 #000;  
    }
    
    th:after {
      background: linear-gradient(rgba(255,255,255,0), rgba(255,255,255,.08));
      content: '';
      display: block;
      height: 25%;
      left: 0;
      margin: 1px 0 0 0;
      position: absolute;
      top: 25%;
      width: 100%;
    }
    
    th:first-child {
      border-left: 1px solid #777;  
      box-shadow: inset 1px 1px 0 #999;
    }
    
    th:last-child {
      box-shadow: inset -1px 1px 0 #999;
    }
    
    td {
      border-right: 1px solid #fff;
      border-left: 1px solid #e8e8e8;
      border-top: 1px solid #fff;
      border-bottom: 1px solid #e8e8e8;
      padding: 10px 15px;
      position: relative;
      transition: all 300ms;
    }
    
    td:first-child {
      box-shadow: inset 1px 0 0 #fff;
    }  
    

    td:last-child {
      border-right: 1px solid #e8e8e8;
      box-shadow: inset -1px 0 0 #fff;
    }  
    
    tr {
      background: url(https://jackrugile.com/images/misc/noise-diagonal.png);  
    }
    
    tr:nth-child(odd) td {
      background: #f1f1f1 url(https://jackrugile.com/images/misc/noise-diagonal.png);  
    }
    
    tr:last-of-type td {
      box-shadow: inset 0 -1px 0 #fff; 
    }
    
    tr:last-of-type td:first-child {
      box-shadow: inset 1px -1px 0 #fff;
    }  
    
    tr:last-of-type td:last-child {
      box-shadow: inset -1px -1px 0 #fff;
    }
    
    tr:hover td {
      background: #e8e8e8;  
    }
    
    tr:nth-child(odd):hover td {
      background: #ddd;  
    }
    
    tr:last-of-type td:hover {
      box-shadow: inset 0 -1px 0 #ddd;  
    }
    
    tr:last-of-type td:first-child:hover {
      box-shadow: inset 1px -1px 0 #ddd;  
    }  
    
    tr:last-of-type td:last-child:hover {
      box-shadow: inset -1px -1px 0 #ddd;  
    }
    
    /* Popup Styles */
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
    animation: fadeIn 0.5s ease-in-out;
  }
  
  /* Popup Content */
  .popup-content {
    background: linear-gradient(to right, #d5fff6, #b7d4f0); /* Linear gradient background */
    padding: 40px 5px;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3); /* Enhanced box shadow */
    width: 400px;
    transform: scale(0.9);
    transition: transform 0.3s ease-in-out;
    animation: scaleIn 0.5s ease-out;
  }
  
  /* Popup Content Animation */
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
  
  .popup-content h3 {
    margin: 0 0 15px;
  }
  
  .popup-content form {
    display: grid;
    gap: 10px;
  }
  
  .popup-content form label {
    font-weight: bold;
  }
  
  .popup-content form input {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    width: 80%; /* Reduce the width and center the inputs */
    margin: 0 auto; /* Center the input fields */
  }
  
  .popup-content form button {
    padding: 10px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    color: white;
    width: 85%;
    margin: 0 auto; /* Center the input fields */
  }
    
    .btn-register {
    font-size: 14px;
    margin-bottom: 20px;
    padding: 10px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 15px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    }
    
    .btn-register:hover {
      background: linear-gradient(to right, #7fffdf, #4685f2);
    }
  
    .btn-register2 {
    font-size: 14px;
    padding: 10px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 15px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    }
    
    .btn-register2:hover {
      background: linear-gradient(to right, #7fffdf, #4685f2);
    }
    
    .btn-edit {
    font-size: 12px;
    padding: 4px 20px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    }
    
    .btn-edit:hover {
      background: linear-gradient(to right, #7fffdf, #4685f2);
    }
  
    .btn-edit2 {
    font-size: 12px;
    padding: 4px 20px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    }
    
    .btn-edit2:hover {
      background: linear-gradient(to right, #7fffdf, #4685f2);
    }
    
    .btn-delete {
    font-size: 12px;
    padding: 4px 12px;
    margin-left: 0px;
    background: linear-gradient(to right, #f6776b, #d21f1f);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
    }
    
    .btn-delete:hover {
      background: linear-gradient(to right, #ffd47f, #f24646);
    }
  
    /* New Cancel Button Style */
  .btn-cancel {
    font-size: 12px;
    padding: 4px 12px;
    background: linear-gradient(to right, #f6776b, #d21f1f);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .btn-cancel:hover {
    background: linear-gradient(to right, #ffd47f, #f24646);
  }
    
  .search-container {
      margin-bottom: 10px;
    }
    
    .search-container input {
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      width: 30%;
    }

    .status-filter {
        padding: 8px;
        border: 1px solid #ccc;
        border-radius: 4px;
        width: 20%;
        margin-left: 10px;
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
    
  
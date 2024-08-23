<template>
    <div class="manage-borrow-return">
      <div class="search-container">
        <input v-model="searchQuery" type="text" placeholder="Search borrow products..." />
        <select v-model="statusFilter" class="status-filter">
        <option value="">Filter by Status</option>
        <option value="Available">Borrowed</option>
        <option value="Unavailable">Out of Stock</option>
      </select>
      </div>
      
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>User Name</th>
              <th>Item Name</th>
              <th>quantity</th>
              <th>Status</th>
              <th>Borrow Date</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in filteredproducts" :key="product.id">
              <td>{{ product.employeeName }}</td>
              <td>{{ product.productName }}</td>
              <td>{{ product.quantity }}</td>
              <td>{{ product.status }}</td>
              <td>{{ new Date(product.borrowedAt).toLocaleString('en-MY') }}</td>
              <td>
                <button v-if="product.status === 'Borrowed'" @click="showReturnPopup(product)" class="btn-return">Return Item</button>
                <span v-if="product.status === 'Returned'" class="status-label">Product is already return</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Return Item Popup -->
      <div v-if="showReturnPopupVisible" class="popup-container">
        <div class="popup-content">
          <h3>Return Item</h3>
          <form @submit.prevent="processReturn">
            <label for="returnquantity">quantity to Return:</label>
            <input v-model.number="returnquantity" id="returnquantity" type="number" min="1" max="100" required />
            
            <button type="submit" class="btn-return2">Return</button>
            <button type="button" class="btn-cancel" @click="showReturnPopupVisible = false">Cancel</button>
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
        products: [], // List of borrow products from the database
        searchQuery: '',
        showReturnPopupVisible: false,
        returnquantity: 0,
        currentproduct: null, // Current product being processed
        notification: {
          visible: false,
          type: '', // 'success' or 'error'
          message: ''
        },
      };
    },
    computed: {
      filteredproducts() {
        return this.products.filter(product => product.employeeName.toLowerCase().includes(this.searchQuery.toLowerCase()) && product.status !== 'Returned');
      }
    },
    methods: {
        async fetchProducts() {
            try {
                const response = await fetch('http://localhost:3000/api/Auth/my-borrowings', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('authToken')}`
                    }
                });
                this.products = await response.json();
            } catch (error) {
                console.error('Failed to fetch products:', error);
            }
        },
    async processReturn() {
        try {
            const response = await fetch('http://localhost:3000/api/Auth/employee-return-product', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({
                    ID: this.currentproduct.id,
                    Quantity: this.returnquantity
                })
            });

            if (response.ok) {
                const data = await response.json();
                this.showNotification('success', data.message);
                this.showReturnPopupVisible = false;
                this.fetchProducts(); // Refresh the product list
            } else {
                const errorData = await response.json();
                this.showNotification('error', errorData.message);
            }
        } catch (error) {
            console.error('Failed to process return:', error);
            this.showNotification('error', 'Failed to process return');
        }
    },

      showReturnPopup(product) {
        this.currentproduct = product;
        this.showReturnPopupVisible = true;
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
    created() {
      this.fetchProducts(); // Fetch products when component is created
    }
  };
  </script>
  
  <style scoped>

    .manage-borrow-return {
        max-width: 800px;
        height: 550px;
        margin: 30px auto;
    }

  .search-container {
    margin-bottom: 5px;
  }
  
  .search-container input {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    width: 60%;
  }

  .table-container {
    max-width: 800px;
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
  
  td {
    border-right: 1px solid #fff;
    border-left: 1px solid #e8e8e8;
    border-top: 1px solid #fff;
    border-bottom: 1px solid #e8e8e8;
    padding: 10px 15px;
    position: relative;
    transition: all 300ms;
  }
  
  tr {
    background: url(https://jackrugile.com/images/misc/noise-diagonal.png);  
  }
  
  tr:nth-child(odd) td {
    background: #f1f1f1 url(https://jackrugile.com/images/misc/noise-diagonal.png);  
  }
  
  tr:hover td {
    background: #e8e8e8;  
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
  
  .btn-return {
    font-size: 12px;
    padding: 4px 20px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .btn-return:hover {
    background: linear-gradient(to right, #7fffdf, #4685f2);
  }
  
  .btn-return2 {
    font-size: 14px;
    padding: 10px;
    background: linear-gradient(to right, #6bc0f6, #1f61d2);
    color: white;
    border: none;
    border-radius: 15px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .btn-return2:hover {
    background: linear-gradient(to right, #7fffdf, #4685f2);
  }
  
  /* New Cancel Button Style */
  .btn-cancel {
    font-size: 12px;
    padding: 8px;
    background: linear-gradient(to right, #f76c6c, #d74d4d);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .btn-cancel:hover {
    background: linear-gradient(to right, #ff6f6f, #e24949);
  }
  
  /* Notification Styles */
  .notification {
    position: fixed;
    top: 20px;
    right: 20px;
    padding: 15px;
    border-radius: 5px;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.3);
    z-index: 1000;
    opacity: 0;
    transition: opacity 0.3s ease;
  }
  
  .notification.success {
    background-color: #d4edda;
    color: #155724;
  }
  
  .notification.error {
    background-color: #f8d7da;
    color: #721c24;
  }
  
  .notification.show {
    opacity: 1;
  }
  
  .notification button {
    background: none;
    border: none;
    font-size: 16px;
    color: inherit;
    cursor: pointer;
  }
  </style>
  
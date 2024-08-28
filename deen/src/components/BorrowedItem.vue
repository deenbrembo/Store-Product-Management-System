<template>
    <div class="history-borrow">
      <!-- Search Controls -->
      <div class="search-container">
        <input v-model="searchQuery" type="text" placeholder="Search by item or user name..." />
      </div>
  
      <!-- Table Container -->
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th @click="sortTable('adminName')">Name</th>
              <th @click="sortTable('productName')">Item Name</th>
              <th @click="sortTable('quantity')">Borrow Quantity</th>
              <th @click="sortTable('borrowedAt')">Borrow Date</th>
              <th @click="sortTable('returnedAt')">Return Date</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in sortedproducts" :key="product.id">
              <td>{{ product.adminName || product.employeeName }}</td>
              <td>{{ product.productName }}</td>
              <td>{{ product.quantity }}</td>
              <td>{{ new Date(product.borrowedAt).toLocaleString('en-MY') }}</td>
              <td>{{ product.returnedAt ? new Date(product.returnedAt).toLocaleString('en-MY') : 'Not Returned' }}</td>
            </tr>
          </tbody>
        </table>
      </div>

              <!-- No Products to Return Popup -->
  <div v-if="filteredproducts.length === 0" class="popup-container">
    <div class="popup-content">
      <h3>No Products are borrowed</h3>
      <p>There are currently no borrowed products..</p>
      <button @click="navigateToBorrowPage" class="btn-borrow">Borrow Products</button>
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
      const config = {
  apiBaseUrl: window.location.hostname === 'localhost' 
    ? 'http://localhost:3000/api' 
    : 'http://192.168.1.14:3000/api'
};
  export default {
    data() {
      return {
        products: [], // List of borrow products from the database
        searchQuery: '',
        sortKey: '', // The current sort key
        sortOrder: 'asc', // 'asc' or 'desc'
        notification: {
          visible: false,
          type: '', // 'success' or 'error'
          message: ''
        }
      };
    },
    computed: {
      filteredproducts() {
        return this.products.filter(product => {
          const matchesSearch = product.adminName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                                product.productName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                                product.employeeName.toLowerCase().includes(this.searchQuery.toLowerCase());
          const matchesStatus = product.status !== 'Returned';
          return matchesSearch && matchesStatus;
        });
      },
      sortedproducts() {
        if (!this.sortKey) return this.filteredproducts;
  
        return this.filteredproducts.slice().sort((a, b) => {
          let compare = 0;
  
          if (a[this.sortKey] > b[this.sortKey]) {
            compare = 1;
          } else if (a[this.sortKey] < b[this.sortKey]) {
            compare = -1;
          }
  
          return this.sortOrder === 'asc' ? compare : -compare;
        });
      }
    },
    methods: {
        async fetchproducts() {
    try {
      const response = await fetch(`${config.apiBaseUrl}/Auth/borrowings`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      this.products = await response.json();
    } catch (error) {
      console.error('Failed to fetch products:', error);
    }
  },
      sortTable(key) {
        if (this.sortKey === key) {
          this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
        } else {
          this.sortKey = key;
          this.sortOrder = 'asc';
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
      },
      navigateToBorrowPage() {
      this.$router.push('borrowing-item'); // Ensure this path matches your router setup
    },
    },
    created() {
      this.fetchproducts(); // Fetch products when component is created
    }
  }
  </script>
  
  <style scoped>

    .history-borrow {
        max-width: 800px;
        height: 550px;
        margin: 30px auto;
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
  padding: 40px 20px;
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
  font-size: 20px;
  margin-bottom: 15px;
  color: #343a40;
  font-weight: bold;
  text-align: center; /* Centered text alignment */
  letter-spacing: 0.5px; /* Slight letter spacing for readability */
}

.popup-content p {
  font-size: 14px;
  color: #666;
  margin-bottom: 20px;
  text-align: center; /* Centered text alignment */
  line-height: 1.5; /* Improved line height for readability */
}

.popup-content .btn-borrow {
  display: block;
  margin: 10px auto 0;
  padding: 8px 16px;
  font-size: 14px;
  background-color: #007bff; /* Blue button background */
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.popup-content .btn-borrow:hover {
  background-color: #0056b3;
}
  </style>
  
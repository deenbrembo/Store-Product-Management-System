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
              <th @click="sortTable('employeeName')">User Name</th>
              <th @click="sortTable('productName')">Item Name</th>
              <th @click="sortTable('quantity')">quantity</th>
              <th @click="sortTable('borrowedAt')">Borrow Date</th>
              <th @click="sortTable('returnedAt')">Return Date</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in sortedproducts" :key="product.id">
              <td>{{ product.employeeName }}</td>
              <td>{{ product.productName }}</td>
              <td>{{ product.quantity }}</td>
              <td>{{ new Date(product.borrowedAt).toLocaleString('en-MY') }}</td>
              <td>{{ product.returnedAt ? new Date(product.returnedAt).toLocaleString('en-MY') : 'Not Returned' }}</td>
            </tr>
          </tbody>
        </table>
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
          const matchesSearch = product.employeeName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                                product.productName.toLowerCase().includes(this.searchQuery.toLowerCase());
          const matchesStatus = product.status !== 'Borrowed';
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
      const response = await fetch('http://localhost:3000/api/Auth/borrowings', {
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
      }
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
  </style>
  
<template>
    <div>
      <div class="search-container">
        <input v-model="searchQuery" type="text" placeholder="Search by Name or Email" />
        <select v-model="statusFilter">
          <option value="">Filter by Status</option>
          <option value="Active">Active</option>
          <option value="Deactivated">Deactivated</option>
        </select>
      </div>
      
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th @click="sortTable('name')">Name</th>
              <th @click="sortTable('email')">Email</th>
              <th @click="sortTable('phoneNumber')">Phone Number</th>
              <th @click="sortTable('role')">Role</th>
              <th @click="sortTable('status')">Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="admin in filteredAdmins" :key="admin.id">
              <td>{{ admin.name || 'N/A' }}</td>
              <td>{{ admin.email || 'N/A' }}</td>
              <td>{{ admin.phoneNumber || 'N/A' }}</td>
              <td>{{ admin.role }}</td>
              <td>{{ admin.status || 'N/A' }}</td>
              <td>
                <button v-if="admin.status === 'Active'" @click="deactivateAdmin(admin)" class="btn-deactivate">Deactivate</button>
                <button v-if="admin.status === 'Deactivated'" @click="activateAdmin(admin)" class="btn-activate">Activate</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <div v-if="notification.show" :class="['notification', notification.type]">
        {{ notification.message }}
        <button @click="notification.show = false">×</button>
      </div>
    </div>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    name: 'ManageAdmin',
    data() {
      return {
        admins: [], // This will be populated from the API
        searchQuery: '',
        statusFilter: '',
        sortColumn: '',
        sortDirection: 'asc',
        notification: {
          show: false,
          message: '',
          type: ''
        }
      };
    },
    computed: {
      filteredAdmins() {
        let result = this.admins;
  
        // Filter by search query
        if (this.searchQuery) {
          result = result.filter(admin =>
            (admin.name && admin.name.toLowerCase().includes(this.searchQuery.toLowerCase())) ||
            (admin.email && admin.email.toLowerCase().includes(this.searchQuery.toLowerCase()))
          );
        }
  
        // Filter by status
        if (this.statusFilter) {
          result = result.filter(admin => admin.status === this.statusFilter);
        }
  
        // Sort
        if (this.sortColumn) {
          result = result.sort((a, b) => {
            const aValue = a[this.sortColumn];
            const bValue = b[this.sortColumn];
            
            if (this.sortDirection === 'asc') {
              return (aValue && aValue > bValue) ? 1 : (aValue < bValue ? -1 : 0);
            } else {
              return (aValue && aValue < bValue) ? 1 : (aValue > bValue ? -1 : 0);
            }
          });
        }
  
        return result;
      }
    },
    methods: {
      async fetchAdmins() {
        try {
          const response = await axios.get('http://localhost:3000/api/Auth/admins', {
            headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
          });
          this.admins = response.data;
        } catch (error) {
          console.error('Error fetching admins:', error);
          this.showNotification('Failed to fetch admins.', 'error');
        }
      },
      
      async deactivateAdmin(admin) {
        try {
          const payload = {
            userId: admin.id
          };
  
          const response = await axios.post('http://localhost:3000/api/Auth/deactivate-admin', payload, {
            headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
          });
  
          if (response.status === 200) {
            this.showNotification(response.data.message, 'success');
            this.fetchAdmins(); // Refresh the admin list to reflect the changes
          } else {
            this.showNotification('Failed to deactivate admin.', 'error');
          }
        } catch (error) {
          console.error('Error deactivating admin:', error);
          this.showNotification('Failed to deactivate admin.', 'error');
        }
      },
  
      async activateAdmin(admin) {
        try {
          const payload = {
            userId: admin.id
          };
  
          const response = await axios.post('http://localhost:3000/api/Auth/activate-admin', payload, {
            headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
          });
  
          if (response.status === 200) {
            this.showNotification(response.data.message, 'success');
            this.fetchAdmins(); // Refresh the admin list to reflect the changes
          } else {
            this.showNotification('Failed to activate admin.', 'error');
          }
        } catch (error) {
          console.error('Error activating admin:', error);
          this.showNotification('Failed to activate admin.', 'error');
        }
      },
      
      showNotification(message, type) {
        this.notification.type = type;
        this.notification.message = message;
        this.notification.show = true;
        setTimeout(() => {
          this.notification.show = false;
        }, 5000);
      },
  
      sortTable(column) {
        if (this.sortColumn === column) {
          this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
          this.sortColumn = column;
          this.sortDirection = 'asc';
        }
      }
    },
    mounted() {
      this.fetchAdmins();
    }
  };
  </script>
  
    
    
    <style scoped>
  
  .btn-approve {
      font-size: 12px;
      padding: 4px 15px;
      background-color: #3879c8;
      color: #fff;
      border: none;
      border-radius: 3px;
      cursor: pointer;
      margin-bottom: 5px;
      transition: background-color 0.3s ease;
    }
    
    .btn-approve:hover {
      background-color: #ffffff;
      color: #3879c8;
      border-color: #3879c8;
    }
  
    .btn-reject {
      font-size: 12px;
      padding: 4px 20px;
      background-color: #df2f2f;
      color: #fff;
      border: none;
      border-radius: 1px;
      cursor: pointer;
      transition: background-color 0.3s ease;
    }
    
    .btn-reject:hover {
      background-color: #ffffff;
      color: #df2f2f;
      border-color: #df2f2f;
    }
  
  
  
    .search-container {
      margin-bottom: 10px;
      margin-top: 56.5px;
    }
    
    .search-container input, .search-container select {
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      margin-right: 10px;
    }
    
    .table-container {
      max-width: 800px;
      margin: 30px auto;
      overflow-y: auto;
      max-height: 400px;
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
      cursor: pointer;
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
  
    .btn-activate {
    font-size: 12px;
    padding: 4px 15px;
    background-color: #28a745;
    color: #fff;
    border: none;
    border-radius: 3px;
    cursor: pointer;
    margin-bottom: 5px;
    transition: background-color 0.3s ease;
  }
  
  .btn-activate:hover {
    background-color: #ffffff;
    color: #28a745;
    border-color: #28a745;
  }
  
  .btn-deactivate {
    font-size: 12px;
    padding: 4px 15px;
    background-color: #dc3545;
    color: #fff;
    border: none;
    border-radius: 3px;
    cursor: pointer;
    margin-bottom: 5px;
    transition: background-color 0.3s ease;
  }
  
  .btn-deactivate:hover {
    background-color: #ffffff;
    color: #dc3545;
    border-color: #dc3545;
  }
    </style>
    
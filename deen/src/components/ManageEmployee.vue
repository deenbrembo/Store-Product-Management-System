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
          <tr v-for="employee in filteredEmployees" :key="employee.id">
            <td>{{ employee.name || 'N/A' }}</td>
            <td>{{ employee.email || 'N/A' }}</td>
            <td>{{ employee.phoneNumber || 'N/A' }}</td>
            <td>{{ employee.role || 'N/A' }}</td>
            <td>{{ employee.status || 'N/A' }}</td>
            <td>
              <button v-if="employee.status === 'Active'" @click="deactivateEmployee(employee)" class="btn-deactivate">Deactivate</button>
              <button v-if="employee.status === 'Deactivated'" @click="activateEmployee(employee)" class="btn-activate">Activate</button>
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

const config = {
  apiBaseUrl: window.location.hostname === 'localhost' 
    ? 'http://localhost:3000/api' 
    : 'http://192.168.1.14:3000/api'
};
export default {
  name: 'ManageEmployee',
  data() {
    return {
      employees: [], // This will be populated from the API
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
    filteredEmployees() {
      let result = this.employees;

      // Filter by search query
      if (this.searchQuery) {
        result = result.filter(employee =>
          (employee.name && employee.name.toLowerCase().includes(this.searchQuery.toLowerCase())) ||
          (employee.email && employee.email.toLowerCase().includes(this.searchQuery.toLowerCase()))
        );
      }

      // Filter by status
      if (this.statusFilter) {
        result = result.filter(employee => employee.status === this.statusFilter);
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
    async fetchEmployees() {
      try {
        const response = await axios.get(`${config.apiBaseUrl}/Auth/employees`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
        });
        this.employees = response.data;
      } catch (error) {
        console.error('Error fetching employees:', error);
        this.showNotification('Failed to fetch employees.', 'error');
      }
    },

    async deactivateEmployee(employee) {
      try {
        const payload = {
          userId: employee.id
        };

        await axios.post(`${config.apiBaseUrl}/Auth/deactivate-employee`, payload, {
          headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
        });

        this.showNotification('Employee deactivated successfully.', 'success');
        this.fetchEmployees(); // Refresh the employee list to reflect the changes
      } catch (error) {
        console.error('Error deactivating employee:', error);
        this.showNotification('Failed to deactivate employee.', 'error');
      }
    },

    async activateEmployee(employee) {
      try {
        const payload = {
          userId: employee.id
        };

        await axios.post(`${config.apiBaseUrl}/Auth/activate-employee`, payload, {
          headers: { Authorization: `Bearer ${localStorage.getItem('authToken')}` }
        });

        this.showNotification('Employee activated successfully.', 'success');
        this.fetchEmployees(); // Refresh the employee list to reflect the changes
      } catch (error) {
        console.error('Error activating employee:', error);
        this.showNotification('Failed to activate employee.', 'error');
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
    this.fetchEmployees();
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
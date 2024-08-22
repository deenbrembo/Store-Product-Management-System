<template>
  <div class="dashboard-container">
    <!-- Sidebar -->
    <div :class="['sidebar', { 'open': isMenuOpen }]">
      <div class="sidebar-inner">
        <div class="sidebar-header">
          <div class="sidebar-burger" @click="toggleMenu">
            <i class='bx bx-menu'></i> <!-- Boxicons Menu Icon -->
          </div>
          <div>
            <img v-if="isMenuOpen" src="@/assets/logo1.png" class="sidebar-logo" alt="Logo">
          </div>
        </div>
        <div class="sidebar-menu">
          <button 
            @click="toggleManagement"
            :class="{ 'active': isManagementActive }"
          >
            <i class='bx bx-cog'></i>
            <span>Management</span>
            <i :class="isManagementActive ? 'bx bx-chevron-down' : 'bx bx-chevron-right'"></i>
          </button>
          <transition name="fade">
            <div v-if="isManagementActive" class="management-menu">
              <button 
                @click="navigate('/headadmin/dashboard/manage-user')"
                :class="{ 'active': isActive('/headadmin/dashboard/manage-user') }"
              >
                <i class='bx bx-minus'></i> <span>Manage User</span>
              </button>
              <button 
                @click="navigate('/headadmin/dashboard/manage-admin')"
                :class="{ 'active': isActive('/headadmin/dashboard/manage-admin') }"
              >
                <i class='bx bx-minus'></i> <span>Manage Admin</span>
              </button>
              <button 
                @click="navigate('/headadmin/dashboard/manage-employee')"
                :class="{ 'active': isActive('/headadmin/dashboard/manage-employee') }"
              >
                <i class='bx bx-minus'></i> <span>Manage Employee</span>
              </button>
              <!-- Moved Manage Profile button here -->
              <button 
                @click="navigate('/headadmin/dashboard/manage-profile')"
                :class="{ 'active': isActive('/headadmin/dashboard/manage-profile') }"
              >
                <i class='bx bx-minus'></i> <span>Manage Profile</span>
              </button>
            </div>
          </transition>
          <button 
            @click="navigate('/headadmin/borrowing-history')"
            :class="{ 'active': isActive('/headadmin/borrowing-history') }"
          >
            <i class='bx bx-history'></i> <span>Borrow History</span>
          </button>
          <button 
            @click="navigate('/headadmin/audit-log')"
            :class="{ 'active': isActive('/headadmin/audit-log') }"
          >
            <i class='bx bx-clipboard'></i> <span>Audit Log</span>
          </button>
          <button class="sidebar-logout" @click="logout">
            <i class='bx bx-log-out'></i> <span>Logout</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <router-view></router-view> <!-- Displays content based on route -->
    </div>
  </div>
</template>

<script>
export default {
  name: 'HeadAdminDashboard',
  data() {
    return {
      isMenuOpen: true,  // Sidebar is open by default
      isManagementActive: false,
    };
  },
  methods: {
    toggleMenu() {
      this.isMenuOpen = !this.isMenuOpen;
      document.body.classList.toggle('open', this.isMenuOpen);
    },
    toggleManagement() {
      this.isManagementActive = !this.isManagementActive;
    },
    navigate(route) {
      this.$router.push(route);
    },
    logout() {
      localStorage.removeItem('authToken');
      this.$router.push('/loginregister');
    },
    isActive(route) {
      return this.$route.path === route;
    },
  },
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Ubuntu:wght@300;400;500;700&display=swap');
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  background: #121212;
}

button {
  background: transparent;
  border: 0;
  padding: 0;
  cursor: pointer;
}

.dashboard-container {
  display: flex;
}

.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  width: 75px;
  height: 100%;
  background: #3949ab;
  transition: all 0.4s;
  overflow-y: auto;
}

body.open .sidebar {
  width: 255px;
}

.sidebar-inner {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
}

.sidebar-header {
  display: flex;
  align-items: center;
  height: 68px;
  padding: 0 1.25rem;
  background: rgba(0, 0, 0, 0.1);
  border-left: 3px solid transparent;
  transition: all 0.4s;
}

.sidebar-header:hover {
  border-left: 3px solid #bdbdbd;
}

.sidebar-burger {
  width: 70px;
  height: 70px;
  display: grid;
  place-items: center;
}

.sidebar-burger > i {
  font-size: 25px;
  color: #f9f9f9;
  transition: all 0.4s;
}

body.open .sidebar-burger > i {
  color: #bdbdbd;
}

.sidebar-logo {
  height: 65px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 25px;
  color: #f9f9f9;
  opacity: 0;
  transition: all 0.4s;
}

body.open .sidebar-logo {
  opacity: 1;
}

.sidebar-menu {
  display: grid;
  gap: 10px;
}

.sidebar-menu > button {
  display: flex;
  justify-content: flex-start; /* Aligns text to the left */
  align-items: center;
  height: 55px;
  font-family: 'Ubuntu', sans-serif;
  font-size: 16px;
  font-weight: 400;
  letter-spacing: 2px;
  line-height: 1;
  padding: 0 22px;
  border-left: 3px solid transparent;
  transition: all 0.4s;
}

.sidebar-menu > button.active {
  background-color: #ffffff0e;
  color: #f9f9f9;
}

.sidebar-menu > button:hover {
  border-left: 3px solid #f9f9f9;
}

.sidebar-menu > button > i {
  font-size: 25px;
  color: #f9f9f9;
  margin-right: 20px; /* Adds space between the icon and the text */
  transition: all 0.4s;
}

.sidebar-menu > button.active > i,
.sidebar-menu > button.active > span {
  color: #bdbdbd;
}

.sidebar-menu > button > span {
  color: #f9f9f9;
  opacity: 0;
  transition: all 0.4s;
}

body.open .sidebar-menu > button > span {
  opacity: 1;
  text-align: left; /* Ensure text is left-aligned */
}

.management-menu {
  display: grid;
  gap: 1px;
  padding-left: 5px;
}

.management-menu > button {
  display: flex;
  justify-content: flex-start; /* Aligns text to the left */
  gap: 12px;
  align-items: center;
  height: 45px;
  font-family: 'Ubuntu', sans-serif;
  font-size: 14px;
  font-weight: 400;
  letter-spacing: 1.5px;
  line-height: 1;
  padding: 0 22px;
  border-left: 3px solid transparent;
  transition: all 0.4s;
}



.management-menu > button.active {
  background-color: #ffffff0e;
  color: #f9f9f9;
}

.management-menu > button:hover {
  border-left: 3px solid #f9f9f9;
}

.management-menu > button > i {
  font-size: 15px;
  color: #f9f9f9;
  margin-right: 20px; /* Adds space between the icon and the text */
  transition: all 0.4s;
}

.management-menu > button.active > i,
.management-menu > button.active > span {
  color: #bdbdbd;
}

.management-menu > button > span {
  color: #f9f9f9;
  opacity: 0;
  transition: all 0.4s;
}

body.open .management-menu > button > span {
  opacity: 1;
  text-align: left; /* Ensure text is left-aligned */
}

.sidebar-logout {
  margin-top: auto; /* Aligns to the bottom */
}

.main-content {
  margin-left: 75px;
  padding: 1rem;
  width: calc(100% - 75px);
  transition: all 0.4s;
}

body.open .main-content {
  margin-left: 255px;
  width: calc(100% - 255px);
}

@media screen and (max-height: 600px) {
  .sidebar {
    height: auto;
    max-height: 100vh;
    overflow-y: scroll;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}

.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>

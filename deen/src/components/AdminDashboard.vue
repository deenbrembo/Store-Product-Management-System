<template>
  <div class="dashboard-container">
    <!-- Sidebar -->
    <div :class="['sidebar', { 'open': isMenuOpen }]">
      <div class="sidebar-inner">
        <div class="sidebar-header">
          <div class="sidebar-burger" @click="toggleMenu">
            <i class='bx bx-menu'></i> <!-- Boxicons Menu Icon -->
          </div>
          <div v-if="isMenuOpen">
            <img src="@/assets/logo1.png" class="sidebar-logo" alt="Logo">
          </div>
        </div>
        <div class="sidebar-menu">
          <button 
            @click="navigate('/admin/dashboard/manage-product')"
            :class="{ 'active': isActive('/admin/dashboard/manage-product') }"
          >
            <i class='bx bx-cart'></i> <span>Manage Item</span>
          </button>
          <button 
            @click="navigate('/admin/dashboard/borrowing-item')"
            :class="{ 'active': isActive('/admin/dashboard/borrowing-item') }"
          >
            <i class='bx bx-cart-add'></i> <span>Borrow Item</span>
          </button>
          <button 
            @click="navigate('/admin/dashboard/return-item')"
            :class="{ 'active': isActive('/admin/dashboard/return-item') }"
          >
            <i class='bx bx-folder-minus'></i> <span>Return Item</span>
          </button>
          <button 
            @click="navigate('/admin/dashboard/borrowed-item')"
            :class="{ 'active': isActive('/admin/dashboard/borrowed-item') }"
          >
            <i class='bx bxs-cart'></i> <span>Borrowed Item</span>
          </button>
          <button 
            @click="navigate('/admin/dashboard/borrow-history')"
            :class="{ 'active': isActive('/admin/dashboard/borrow-history') }"
          >
            <i class='bx bx-history'></i> <span>Borrow History</span>
          </button>
          <button 
            @click="navigate('/admin/dashboard/manage-profile')"
            :class="{ 'active': isActive('/admin/dashboard/manage-profile') }"
          >
            <i class='bx bx-user'></i> <span>Manage Profile</span>
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
    
    <!-- Hamburger Button for Mobile -->
    <div class="mobile-menu-toggle" @click="toggleMenu">
      <i class='bx bx-menu'></i> <!-- Boxicons Menu Icon -->
    </div>
  </div>
</template>

<script>
export default {
  name: 'adminDashboard',
  data() {
    return {
      isMenuOpen: false,  // Sidebar is closed by default on mobile
    };
  },
  methods: {
    toggleMenu() {
      this.isMenuOpen = !this.isMenuOpen;
      document.body.classList.toggle('open', this.isMenuOpen);
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
  font-family: 'Ubuntu', sans-serif;
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
  z-index: 1000; /* Ensure sidebar is above other content */
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
  align-items: center;
  height: 55px;
  font-size: 16px;
  font-weight: 400;
  letter-spacing: 2px;
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
  margin-right: 20px;
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
  text-align: left;
}

.sidebar-logout {
  margin-top: auto;
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

.mobile-menu-toggle {
  display: none;
  position: fixed;
  top: 10px;
  left: 10px;
  font-size: 25px;
  color: #f9f9f9;
  cursor: pointer;
  z-index: 1000; /* Ensure it's above other content */
}

@media screen and (max-width: 768px) {
  .sidebar {
    width: 0;
    display: none; /* Hide the sidebar on mobile by default */
  }

  body.open .sidebar {
    display: block; /* Show sidebar when open on mobile */
  }

  .mobile-menu-toggle {
    display: block; /* Show the hamburger button on mobile */
    color: #3949ab;
  }

  body.open .main-content {
    margin-left: 0;
    width: 100%;
  }
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

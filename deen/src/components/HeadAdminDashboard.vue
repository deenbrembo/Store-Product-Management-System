<template>
  <div :class="['dashboard-container', { 'dark-mode': isDarkMode }]">
    <!-- Sidebar -->
    <div :class="['sidebar', { 'open': isMenuOpen }]">
      <div class="sidebar-inner">
        <div class="sidebar-header">
          <div class="sidebar-burger" @click="toggleMenu">
            <i class='bx bx-menu'></i>
          </div>
          <div>
            <img v-if="isMenuOpen" src="@/assets/logo1.png" class="sidebar-logo" alt="Logo">
          </div>
        </div>
        <div class="sidebar-menu">
          <button 
            @click="navigate('/headadmin/dashboard/manage-admin')"
            :class="{ 'active': isActive('/headadmin/dashboard/manage-admin') }"
          >
            <i class='bx bx-user'></i> <span>Manage Admins</span>
          </button>
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
          <button 
            @click="navigate('/headadmin/manage-profile')"
            :class="{ 'active': isActive('/headadmin/manage-profile') }"
          >
            <i class='bx bx-user-circle'></i> <span>Manage Profile</span>
          </button>
          <button class="sidebar-logout"  @click="logout">
            <i class='bx bx-log-out'></i> <span>Logout</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Top Bar -->
    <div :class="['top-bar', { 'sidebar-open': isMenuOpen }]">
      <button class="top-bar-icon" @click="toggleDarkMode">
        <i :class="isDarkMode ? 'bx bx-sun' : 'bx bx-moon'"></i>
      </button>
      <button class="top-bar-icon" @click="toggleFullscreen">
        <i :class="isFullscreen ? 'bx bx-exit-fullscreen' : 'bx bx-fullscreen'"></i>
      </button>
      <button class="top-bar-icon" @click="toggleNotifications">
        <i class='bx bx-bell'></i>
        <span v-if="notifications.length" class="notification-badge">{{ notifications.length }}</span>
      </button>
    </div>

    <!-- Notifications -->
    <transition name="slide-fade">
      <div v-if="showNotifications" class="notifications">
        <div v-for="(notification, index) in notifications" :key="index" class="notification-item">
          <span>{{ notification }}</span>
          <button class="notification-close" @click="removeNotification(index)">&times;</button>
        </div>
      </div>
    </transition>

    <!-- Main Content -->
    <div class="main-content">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
export default {
  name: 'HeadAdminDashboard',
  data() {
    return {
      isMenuOpen: false,
      isDarkMode: false,
      isFullscreen: false,
      showNotifications: false,
      notifications: ['New admin added', 'System update available']
    };
  },
  methods: {
  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
    document.body.classList.toggle('open', this.isMenuOpen);
    const topBar = document.querySelector('.top-bar');
    if (this.isMenuOpen) {
      topBar.classList.add('top-bar-moved');
    } else {
      topBar.classList.remove('top-bar-moved');
    }
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
  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
  },
  toggleFullscreen() {
    if (this.isFullscreen) {
      document.exitFullscreen();
    } else {
      document.documentElement.requestFullscreen();
    }
    this.isFullscreen = !this.isFullscreen;
  },
  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
  },
  removeNotification(index) {
    this.notifications.splice(index, 1);
  }
}
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
}

.dark-mode {
  background: #121212;
  color: #f9f9f9;
}

button {
  background: transparent;
  border: 0;
  padding: 0;
  cursor: pointer;
}

.dashboard-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  margin-left: 75px; /* Adjust for sidebar width */
}

.dashboard-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  position: relative; /* Needed for absolute positioning of child elements */
}

.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  width: 75px;
  height: 100%;
  background: #3949ab;
  transition: all 0.4s;
  overflow: hidden;
  z-index: 100; /* Ensures sidebar stays on top of content */
}

.sidebar.open {
  width: 240px;
}

.sidebar-inner {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  height: 68px;
  padding: 0 1.25rem;
  background: rgba(0, 0, 0, 0.1);
  border-left: 3px solid transparent;
  transition: all 0.4s;
}

.sidebar-menu {
  display: flex;
  flex-direction: column;
  margin-top: 20px; /* Space from the top header */
}

body.open .sidebar {
  width: 240px;
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
}

.sidebar-menu > button {
  display: flex;
  gap: 12px;
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
  background-color: #ffffff13;
  color: #f9f9f9;
}

.sidebar-menu > button:hover {
  border-left: 3px solid #f9f9f9;
}

.sidebar-menu > button.has-border {
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.12);
  margin-bottom: 1rem;
}

.sidebar-menu > button > i {
  font-size: 25px;
  color: #f9f9f9;
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
}

.top-bar {
  position: fixed;
  top: 0;
  left: 75px; /* Offset for the sidebar */
  width: calc(100% - 75px); /* Full width minus sidebar width */
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: 0 1.25rem;
  background: #3949ab;
  color: #f9f9f9;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.4s;
  z-index: 100; /* Ensures top bar stays on top of content */
}

.top-bar-moved {
  left: 240px; /* Adjust this value to match the expanded width of the sidebar */
  width: calc(100% - 240px);
}

.top-bar-icon {
  font-size: 20px;
  margin-left: 20px;
}

.notification-badge {
  position: absolute;
  top: 10px;
  right: 46px;
  background: red;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
}

.notifications {
  position: fixed;
  top: 60px; /* Adjusted for top bar height */
  right: 10px;
  width: 300px;
  background: #fff;
  border-left: 1px solid #ddd;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  max-height: 400px;
  overflow-y: auto;
  transition: transform 0.3s ease;
  transform: translateY(-100%);
}

.notifications.show {
  transform: translateY(0);
}

.notification-item {
  padding: 10px;
  border-bottom: 1px solid #ddd;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.notification-close {
  background: transparent;
  border: none;
  font-size: 18px;
  cursor: pointer;
}

/* Slide fade transition */
.slide-fade-enter-active, .slide-fade-leave-active {
  transition: opacity 0.5s, transform 0.5s;
}
.slide-fade-enter, .slide-fade-leave-to /* .slide-fade-leave-active in <2.1.8 */ {
  opacity: 0;
  transform: translateY(-20px);
}
</style>



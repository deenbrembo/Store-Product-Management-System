<template>
    <div class="container" id="container">
      <!-- Toggle Container -->
      <div class="toggle-container">
        <div class="toggle">
          <div class="toggle-panel toggle-left">
            <h2>Login</h2>
            <p>Already have an account?</p>
            <button id="login">Login</button>
          </div>
          <div class="toggle-panel toggle-right">
            <h2>Register</h2>
            <p>Don't have an account?</p>
            <button id="register">Register</button>
            <p class="switch-login">
            </p>
          </div>
        </div>
      </div>
      
      <!-- Form Container -->
      <div class="form-container sign-in">
        <form @submit.prevent="login">
          <h2>Login</h2>
          <input type="email" v-model="loginEmail" placeholder="Email" required />
          <input type="password" v-model="loginPassword" placeholder="Password" required />
          <button type="submit">Login</button>
        </form>
      </div>
      
      <div class="form-container sign-up">
        <form @submit.prevent="register">
          <h2>Register</h2>
          <input type="text" v-model="registerName" placeholder="Name" required />
          <input type="email" v-model="registerEmail" placeholder="Email" required  />
          <input type="text" v-model="registerPhoneNumber" placeholder="Phone Number" />
          <input type="password" v-model="registerPassword" placeholder="Password" required />
          <input type="password" v-model="registerConfirmPassword" placeholder="Confirm Password" required />
          <button type="submit">Register</button>
        </form>
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
  import axios from 'axios';

  
  export default {
    data() {
      return {
        loginEmail: '',
        loginPassword: '',
        registerName: '',
        registerEmail: '',
        registerPassword: '',
        registerPhoneNumber: '',
        registerConfirmPassword: '',
        notification: {
          visible: false,
          type: '',
          message: ''
        }
      };
    },
    methods: {
      async login() {
        try {
          const response = await axios.post('http://localhost:3000/api/Auth/login', {
        email: this.loginEmail,
        password: this.loginPassword
          });
          const { token, role, status } = response.data;
          localStorage.setItem('authToken', token);
          if (role === 'HeadAdmin') {
        // Show success notification
        this.showNotification('success', 'Login successful! Redirecting...');
        // Redirect to user dashboard
        setTimeout(() => {
          this.$router.push('/headadmin/dashboard'); // Adjust route as necessary
        }, 2000); // Adjust the delay as needed
          } else if (role === 'Admin') {
        if (status === 'Pending') {
          this.showNotification('info', 'Your status is still pending. Please wait for HeadAdmin approval.');
        } else if (status === 'Active') {
          // Show success notification
          this.showNotification('success', 'Login successful! Redirecting...');
          // Redirect to user dashboard
          setTimeout(() => {
            this.$router.push('/admin/dashboard'); // Adjust route as necessary
          }, 2000); // Adjust the delay as needed
        } else if (status === 'Rejected') {
          this.showNotification('error', 'Your account is rejected by Admin. Please contact the HeadAdmin.');
        } else if (status === 'Inactive') {
          this.showNotification('error', 'Your account is deactivated by Admin. Please contact the HeadAdmin.');
        }
          } else if (role === 'Employee') {
        if (status === 'Pending') {
          this.showNotification('info', 'Your status is still pending. Please wait for Admin approval.');
        } else if (status === 'Active') {
          // Show success notification
          this.showNotification('success', 'Login successful! Redirecting...');
          // Redirect to user dashboard
          setTimeout(() => {
            this.$router.push('/employee/dashboard'); // Adjust route as necessary
          }, 2000); // Adjust the delay as needed
        } else if (status === 'Rejected') {
          this.showNotification('error', 'Your account is rejected by Admin. Please contact the Admin.');
        } else if (status === 'Inactive') {
          this.showNotification('error', 'Your account is deactivated by Admin. Please contact the Admin.');
        }
          }
        } catch (error) {
          this.showNotification('error', 'Login failed: ' + error.response.data.message);
        }
      },
      async register() {
      // Check if passwords match
      if (this.registerPassword !== this.registerConfirmPassword) {
        this.showNotification('error', 'Passwords do not match.');
        return;
      }

      try {
        // Send registration request to the server
        const response = await axios.post('http://localhost:3000/api/Auth/register', {
          name: this.registerName,
          email: this.registerEmail,
          phoneNumber: this.registerPhoneNumber,
          password: this.registerPassword
        });

        // Show success notification
        this.showNotification('success', response.data.message);

        // Reset the form
        this.resetForm();

        // Optionally redirect to login page
        setTimeout(() => {
          this.$router.push('/login'); // Redirect to login page or other desired page
        }, 2000);
      } catch (error) {
        // Show error notification
        this.showNotification('error', 'Registration failed: ' + error.response.data.message);
      }
    },
      showNotification(type, message) {
        this.notification.type = type;
        this.notification.message = message;
        this.notification.visible = true;
        setTimeout(() => {
          this.closeNotification();
        }, 5000);
      },
      closeNotification() {
        this.notification.visible = false;
      },
      resetForm() {
      this.registerName = '';
      this.registerEmail = '';
      this.registerPhoneNumber = '';
      this.registerPassword = '';
      this.registerConfirmPassword = '';
      this.registerRole = '';
    },
    },
    mounted() {
      const container = document.getElementById('container');
      const registerBtn = document.getElementById('register');
      const loginBtn = document.getElementById('login');

      
      registerBtn.addEventListener('click', () => {
        container.classList.add("active");
      });
  
      loginBtn.addEventListener('click', () => {
        container.classList.remove("active");
      });
    }
  };
  </script>
  
  
  <style scoped>
  /* Add the CSS styles provided in your question here */
  @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&display=swap');
  
  * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
      font-family: 'Montserrat', sans-serif;
  }
  
  body {
      background-color: #c9d6ff;
      background: linear-gradient(to right, #e2e2e2, #c9d6ff);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      height: 100vh;
  }
  
  .container {
      background-color: #fff;
      border-radius: 30px;
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.35);
      position: relative;
      overflow: hidden;
      width: 768px;
      max-width: 100%;
      min-height: 480px;
  }
  
  .container p {
      font-size: 14px;
      line-height: 20px;
      letter-spacing: 0.3px;
      margin: 20px 0;
  }
  
  .container span {
      font-size: 12px;
  }
  
  .container a {
      color: #333;
      font-size: 13px;
      text-decoration: none;
      margin: 15px 0 10px;
  }
  
  .container button {
      background-color: #512da8;
      color: #fff;
      font-size: 12px;
      padding: 10px 45px;
      border: 1px solid transparent;
      border-radius: 8px;
      font-weight: 600;
      letter-spacing: 0.5px;
      text-transform: uppercase;
      margin-top: 10px;
      cursor: pointer;
  }

  .container button:hover {
      background-color: #ffffff;
      color: #512da8;
      border-color: #512da8;
  }
  
  .container button.hidden {
      background-color: transparent;
      border-color: #fff;
  }
  
  .container form {
      background-color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      padding: 0 40px;
      height: 100%;
  }
  
  .container input, .container select { /* Added select */
      background-color: #eee;
      border: none;
      margin: 8px 0;
      padding: 10px 15px;
      font-size: 13px;
      border-radius: 8px;
      width: 100%;
      outline: none;
  }
  
  .form-container {
      position: absolute;
      top: 0;
      height: 100%;
      transition: all 0.6s ease-in-out;
  }
  
  .sign-in {
      left: 0;
      width: 50%;
      z-index: 2;
  }
  
  .container.active .sign-in {
      transform: translateX(100%);
  }
  
  .sign-up {
      left: 0;
      width: 50%;
      opacity: 0;
      z-index: 1;
  }
  
  .container.active .sign-up {
      transform: translateX(100%);
      opacity: 1;
      z-index: 5;
      animation: move 0.6s;
  }
  
  @keyframes move {
      0%, 49.99% {
          opacity: 0;
          z-index: 1;
      }
      50%, 100% {
          opacity: 1;
          z-index: 5;
      }
  }
  
  .social-icons {
      margin: 20px 0;
  }
  
  .social-icons a {
      border: 1px solid #ccc;
      border-radius: 20%;
      display: inline-flex;
      justify-content: center;
      align-items: center;
      margin: 0 3px;
      width: 40px;
      height: 40px;
  }
  
  .toggle-container {
      position: absolute;
      top: 0;
      left: 50%;
      width: 50%;
      height: 100%;
      overflow: hidden;
      transition: all 0.6s ease-in-out;
      border-radius: 150px 0 0 100px;
      z-index: 1000;
  }
  
  .container.active .toggle-container {
      transform: translateX(-100%);
      border-radius: 0 150px 100px 0;
  }
  
  .toggle {
      background-color: #512da8;
      height: 100%;
      background: linear-gradient(to right, #7135ff, #7135ff);
      color: #fff;
      position: relative;
      left: -100%;
      height: 100%;
      width: 200%;
      transform: translateX(0);
      transition: all 0.6s ease-in-out;
  }
  
  .container.active .toggle {
      transform: translateX(50%);
  }
  
  .toggle-panel {
      position: absolute;
      width: 50%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      padding: 0 30px;
      text-align: center;
      top: 0;
      transform: translateX(0);
      transition: all 0.6s ease-in-out;
  }
  
  .toggle-left {
      transform: translateX(-200%);
  }
  
  .container.active .toggle-left {
      transform: translateX(0);
  }
  
  .toggle-right {
      right: 0;
      transform: translateX(0);
  }
  
  .container.active .toggle-right {
      transform: translateX(200%);
  }

  .switch-login {
      margin-top: 20px;
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
  
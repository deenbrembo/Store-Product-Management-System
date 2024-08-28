<template>
    <div class="container">
      <form @submit.prevent="updateProfile">
        <h2>Manage Profile</h2>
  
        <div class="form-group">
          <label for="name">Name:</label>
          <input
            type="text"
            id="name"
            v-model="form.name"
            placeholder="Enter your name"
          />
        </div>
  
        <div class="form-group">
          <label for="email">Email:</label>
          <input
            type="email"
            id="email"
            v-model="form.email"
            placeholder="Enter your email"
          />
        </div>
  
        <div class="form-group">
          <label for="phoneNumber">Phone Number:</label>
          <input
            type="text"
            id="phoneNumber"
            v-model="form.phoneNumber"
            placeholder="Enter your phone number"
          />
        </div>
  
        <div class="form-group">
          <label for="password">Password:</label>
          <input
            type="password"
            id="password"
            v-model="form.password"
            placeholder="Enter a new password"
          />
        </div>
  
        <button type="submit">Update Profile</button>
      </form>
  
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
  import axios from "axios";

  const config = {
  apiBaseUrl: window.location.hostname === 'localhost' 
    ? 'http://localhost:3000/api' 
    : 'http://192.168.1.14:3000/api'
};
  export default {
    data() {
      return {
        form: {
          name: "",
          email: "",
          phoneNumber: "",
          password: "",
        },
        notification: {
          show: false,
          message: "",
          type: "",
        },
      };
    },
    methods:{
    async updateProfile() {
      try {
        const confirmed = confirm("Are you sure you want to update your profile?");
        if (!confirmed) {
          return;
        }
        
        const response = await axios.post(`${config.apiBaseUrl}/Auth/manage-profile`, this.form, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('authToken')}`,
          },
        });
        this.showNotification('success', response.data.message);
        alert(response.data.message);
        this.clearForm(); // Add this line to clear the form
      } catch (error) {
        console.error(error);
        this.showNotification('error', 'An error occurred while updating the profile.');
      }
    },
    clearForm() {
      this.form.name = "";
      this.form.email = "";
      this.form.phoneNumber = "";
      this.form.password = "";
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
    },
  };
  </script>
  
  <style scoped>
  @import url("https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&display=swap");
  
  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: "Montserrat", sans-serif;
  }
  
  body {
    background: linear-gradient(to right, #e2e2e2, #c9d6ff);
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100vh;
  }
  
  .container {
    background-color: #fff;
    border-radius: 30px;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.35);
    padding: 40px;
    width: 170%;
    right: 15%;
    position: relative;
  }
  h2 {
    margin-bottom: 20px;
    font-size: 24px;
    font-weight: 600;
    color: #333;
    text-align: center;
  }
  
  .form-group {
    margin-bottom: 15px;
  }
  
  label {
    display: block;
    margin-bottom: 5px;
    font-weight: 500;
  }
  
  input {
    background-color: #eee;
    border: none;
    padding: 10px;
    font-size: 14px;
    border-radius: 8px;
    width: 100%;
    outline: none;
  }
  
  button {
    background-color: #512da8;
    color: #fff;
    padding: 12px;
    border: none;
    border-radius: 8px;
    font-size: 14px;
    font-weight: 600;
    width: 100%;
    cursor: pointer;
    transition: background-color 0.3s ease;
    text-transform: uppercase;
    margin-top: 20px;
  }
  
  button:hover {
    background-color: #ffffff;
    color: #512da8;
    border: 1px solid #512da8;
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
  
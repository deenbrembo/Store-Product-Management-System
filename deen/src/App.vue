<template>
  <router-view></router-view>
</template>

<script>
export default {
  name: 'App'
};
</script>


<style>
/* Global styles for the App */
/* Global styles for the App */
/* Global styles for the App */
body, html {
  height: 100%;
  margin: 0;
  overflow: hidden; /* Prevent scrolling on body if you want the image to be fixed */
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #192f46;
  background-color: #fbf9ff;
  height: 100vh; /* Full viewport height */
  width: 100vw; /* Full viewport width */
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: auto; /* Allow scrolling inside the container */
}

</style>

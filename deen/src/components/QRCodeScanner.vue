<template>
    <div class="scanner-container">
      <qrcode-stream @decode="onDecode" @error="onError" />
      <button @click="$emit('close')">Close</button>
    </div>
  </template>
  
  <script>
  import { QrcodeStream } from 'vue-qrcode-reader';
  
  export default {
    components: {
      QrcodeStream
    },
    methods: {
      onDecode(result) {
        this.$emit('scan', result); // Emit the scan result to the parent
      },
      onError(error) {
        console.error('QR Code scan error:', error);
      }
    }
  }
  </script>
  
  <style scoped>
  .scanner-container {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .scanner-container button {
    position: absolute;
    top: 20px;
    right: 20px;
    padding: 10px 20px;
    background-color: #f63232;
    color: #fff;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  </style>
  
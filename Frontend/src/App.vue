<script>
import Application from './components/Application.vue';
export default {
  name: 'App',
  components: {
    Application,
  },
};
</script>

<template>
  <div class="container">
  <div class="row">
    <div class="col-md-12">
      <Application></Application>
    </div>
  </div>
  </div>
</template>

<style>
#app {
  font-family: Arial, Helvetica, sans-serif;
  text-align: center;
  margin-top: 60px;
}
body {
  background-color: rgba(28, 27, 25, 0.8);
  
}
</style>

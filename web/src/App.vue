<template>
  <div id="app" @click="updateLastTime()">
    <router-view/>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  name: 'App',
  computed: {
    ...mapGetters([
      'oidcIsAuthenticated'
    ])
  },
  methods: {
    userLoaded: function (e) {
      console.log('I am listening to the user loaded event in vuex-oidc', e.detail)
    },
     updateLastTime() {
    this.$store.commit('login/SET_LASTTIME'
      , new Date().getTime());
    // console.log(this.$store.state.login.lastTime)
  }
  },
  mounted() {
    window.addEventListener('vuexoidc:userLoaded', this.userLoaded)
  },
  destroyed() {
    window.removeEventListener('vuexoidc:userLoaded', this.userLoaded)
  },
 
}
</script>

<template>
  <div class="dashboard-container">
    <component :is="currentRole"></component>
  </div>
</template>

<script>

import adminDashboard from './admin'


export default {
  name: 'dashboard',
  components: { adminDashboard },
  data() {
    return {
      currentRole: 'adminDashboard',
      // 上一次点击时间
      lastTime: null,
      // 当前时间
      currentTime: null,
      // 超时时间, 如果3分钟都没有点击页面就算超时
      sys_timeout: 3*60 * 1000,
      // 检查的时间，每隔1分钟检查一次有没有超时
      check_time: 60 * 1000,
      // 计时器
      whole_timer: null
    }
  },
  created() {
    // 启动这个定时器  
    this.isLoginOut();
  },
  methods: {
    // 判断是否超时
    isTimeOut() {
      // 页面上一次的点击时间
      this.lastTime = this.$store.state.login.lastTime;
      // console.log(this.$store.state.login.lastTime);
      this.currentTime = new Date().getTime();
      // 超时了
      if ((this.currentTime - this.lastTime) > this.sys_timeout) {
        return true;
      } else {
        return false;
      }
    },
    isLoginOut() {
      
      // console.log("11111:" + this)
      // 每次用定时器之前先清除一下  
      clearInterval(this.whole_timer);
      // 这里要备份一下this指针
      let _this = this;
      this.whole_timer = setInterval(function () {
        console.log(_this.isTimeOut())
        // 判断一下是否超时，如果超时了就退出
        if (_this.isTimeOut()) {
          // console.log("22222:" + this)
          
          _this.$store.dispatch('LogOut').then(() => {

            location.reload() // 为了重新实例化vue-router对象 避免bug
          })
          // 如果退出了就清除这个定时器，这里要延迟一秒清除，因为跳转到登录界面会有一点点延迟~  
          setTimeout(function () {
            clearInterval(_this.whole_timer);
          }, 1000);
        }
      }, _this.check_time);
    }
  }

}
</script>
  
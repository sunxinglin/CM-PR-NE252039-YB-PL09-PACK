<template>
    <div class="login-container" @click="pwdFocus">
        <div class="content">
            <img class="leftImg" src="~@/assets/login/left.png" alt="">
            <el-tabs tabPosition="left" class="login-tabs">

                <h3 class="title" style="font-size:50px">CATL</h3>
                <!-- <label
                    style="position: fixed; height: 70px; width: 450px; z-index: 1; background: #EBEBEA; padding-top: 25px; font-size: 60px; "
                    @click="pwdFocus">请刷卡登录</label> -->
                <el-form autoComplete="on" :model="loginpaycard" class="login-form" @submit.native.prevent
                    label-position="left">

                    <el-form-item>
                        <el-input type="password" autoComplete="on" id="txtPWD" autofocus="true"
                            v-model="loginpaycard.username" />
                    </el-form-item>
                    <el-button @click="cardenter">LOGIN</el-button>
                </el-form>




            </el-tabs>

        </div>

    </div>
</template>

<script>
import waves from '@/directive/waves' // 水波纹指令
import { Console } from 'console'
import AppVue from '../../App.vue'
export default {
    name: 'login',
    directives: {
        waves
    },
    created() {
        var _self = this

        document.onkeydown = function (e) {
            var key = window.event.keyCode
            if (key === 13) {
                var strLastKeyTime = localStorage.getItem('lastKeyTime');
                if (strLastKeyTime != "" && strLastKeyTime != undefined) {
                    var lastTIme = parseInt(localStorage.getItem('lastKeyTime'));
                    localStorage.removeItem("lastKeyTime");
                    var nowTime = parseInt(new Date().getTime());
                    console.log(nowTime - lastTIme);

                    if (nowTime - lastTIme > 40) {
                        _self.loginpaycard.username = "";
                        console.log("#########################");
                    }
                    else {

                        _self.cardenter();
                        _self.loginpaycard.username = "";
                    }
                }

            }
            else {
                var now = new Date().getTime();
                localStorage.setItem('lastKeyTime', now);
            }
        }
    },
    beforeDestroy() {
        document.onkeydown = function (e) {
            var key = window.event.keyCode

            if (key === 13) {
                console.log(1);
            }
        }
    },
    data() {
        const validateUsername = (rule, value, callback) => {
            if (value.length <= 0) {
                callback(new Error('用户名不能为空'))
            } else {
                callback()
            }
        }
        const validatePass = (rule, value, callback) => {
            if (value.length <= 0) {
                callback(new Error('密码不能为空'))
            } else {
                callback()
            }
        }
        return {

            loginForm: {
                username: '123456',
                password: '123456'
            },
            loginpaycard: {
                username: '',
                password: ''
            },
            loginRules: {
                username: [{
                    required: true,
                    trigger: 'blur',
                    validator: validateUsername
                }],
                password: [{
                    required: true,
                    trigger: 'blur',
                    validator: validatePass
                }]
            },
            loading: false,
            pwdType: 'password'
        }
    },
    computed: {

    },
    watch: {
        box() {
            if (this.box == true) {
                this.$nextTick(() => {
                    this.$refs.input.$refs.input.focus();
                })
            }
        }
    },
    mounted() {
        this.pwdFocus();
        //    //this.$refs.TXTpassword.focus();
        //    document.getElementById("TXTpassword").focus();
        //    console.log("11111111111");
    },
    methods: {
        pwdFocus() {
            document.getElementById("txtPWD").focus();
        },
        showPwd() {
            if (this.pwdType === 'password') {
                this.pwdType = ''
            } else {
                this.pwdType = 'password'
            }
        },
        handleLogin() {
            this.$refs.loginForm.validate(valid => {
                if (valid) {
                    this.loading = true
                    this.$store.dispatch('Login', this.loginForm).then(() => {
                        this.loading = false
                        sessionStorage.setItem("username", this.loginForm.username)
                        console.log(sessionStorage.getItem("username"))
                        this.$router.push({
                            path: '/'
                        })
                    }).catch(() => {
                        this.loading = false
                    })
                } else {
                    console.log('error submit!!')
                    return false
                }
            })
        }
        ,
        cardenter() {

            this.loginpaycard.password = this.loginpaycard.username
            this.$store.dispatch('Login', this.loginpaycard).then(() => {

                this.loading = false
                this.$router.push({
                    path: '/'
                })
            }).catch(() => {
                this.loading = false
            })
        }

    }
}

</script>

<style rel="stylesheet/scss" lang="scss">
$bg: #2d3a4b;
$light_gray: #eee;
$color_balck: #333;

/* reset element-ui css */
.login-container {
    .el-input {
        display: inline-block;
        height: 47px;
        width: 85%;

        input {
            background: transparent;
            border: 0px;
            -webkit-appearance: none;
            border-radius: 0px;
            padding: 12px 5px 12px 15px;
            color: $color_balck;
            height: 47px;

            &:-webkit-autofill {
                transition: background-color 5000s ease-in-out 0s;
            }
        }
    }

    .el-form-item {
        margin-bottom: 35px;
        border-radius: 5px;
        color: #454545;

        .el-form-item__content {
            background: #fff;
            border: 1px solid rgba(223, 223, 223, 1);
        }

        &:last-child {
            padding-top: 20px;

            .el-form-item__content {
                border: none;
            }
        }
    }
}
</style>

<style rel="stylesheet/scss" lang="scss" scoped>
@media screen and (max-width: 1150px) {
    .leftImg {
        width: 450px !important;
    }
}

@media screen and (max-width: 1010px) {
    .leftImg {
        width: 380px !important;
    }
}

@media screen and (max-width: 940px) {
    .leftImg {
        display: block;
        width: 260px !important;
        margin: 0 auto !important;
    }
}

$dark_gray: #D1DFE8;

.login-container {
    height: 100%;
    background: url('~@/assets/login/bg.png') no-repeat;
    background-color: #EBEBEA;
    background-position: 0 0;
    background-size: 62% 100%;
    text-align: center;

    &:before {
        content: '';
        display: inline-block;
        height: 100%;
        vertical-align: middle;
    }

    .content {
        display: inline-block;
        vertical-align: middle;

        >img {
            width: 568px;
            margin-right: 150px;
            vertical-align: middle;
        }

        .login-form {
            display: inline-block;
            width: 400px;
            vertical-align: middle;
            height: 400px
        }

        .login-tabs {
            display: inline-block;
            width: 600px;
            vertical-align: middle;
        }
    }

    .svg-container {
        color: $dark_gray;
        vertical-align: middle;
        width: 33px;
        display: inline-block;
        font-size: 22px;

        &_login {
            font-size: 31px;
        }
    }

    .title {
        font-size: 34px;
        font-weight: 400;
        color: #4452D5;
        margin: 0px 0px 40px 0px;
    }

    .tips {
        color: #959595;
        font-size: 14px;
        margin-top: 0;
        margin-bottom: 40px;
        text-align: left;
    }

    .show-pwd {
        position: absolute;
        right: 10px;
        top: 7px;
        font-size: 16px;
        color: $dark_gray;
        cursor: pointer;
        user-select: none;
        font-size: 24px;
    }
}
</style>

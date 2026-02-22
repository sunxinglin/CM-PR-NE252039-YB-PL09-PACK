import { isIdentityAuth } from '@/api/serverConf'

const serverConf = {
  state: {
    isIdentityAuth: undefined
  },
  mutations: {
    SET_IDENTITYAUTH: (state,isIdentityAuth) => {
      state.isIdentityAuth = isIdentityAuth
    }
  },
  actions: {
   
  }
}

export default serverConf

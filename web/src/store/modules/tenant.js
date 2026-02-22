const tenant = {
  state: {
    id: 'OpenAuthDBContext' // 默认租户Id
  },

  mutations: {
    SET_ID: (state, id) => {
      state.id = id
    }
  },

  actions: {
    setTenantId: ({ commit }, id) => {
      commit('SET_ID',id)
    },
 
  },
  alivetime:{
    newtime:-1,
    oldalivetime:0
  },
 
}

export default tenant

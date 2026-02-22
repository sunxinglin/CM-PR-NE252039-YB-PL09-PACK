import request from '@/utils/request'
import { getToken } from '@/utils/auth' // 验权



export function login(username, password) {
  return request({
    url: '/Account/Login',
    method: 'post',
    data: {
      Account: username,
      Password: password
    }
  })
}

export function getInfo(token) {
  return request({
    url: '/User/Getusername',
    method: 'get',
    params: { token }
  })
}

export function getUserProfile() {
  return request({
    url: '/check/getuserprofile',
    method: 'get',
    params: { token: getToken() }
  })
}

export function getModules() {
  return request({
    url: '/Modules/GetList',
    method: 'get',
    params: { token: getToken() }
  })
}

export function getFuncModuleList() {
  return request({
    url: '/Modules/GetFuncModuleList',
    method: 'get',
    params: { token: getToken() }
  })
}



export function getModulesTree() {
  return request({
    url: '/Modules/GetList',
    method: 'get',
    params: { token: getToken() }
  })
}

// export function getOrgs() {
//   return request({
//     url: '/check/getorgs',
//     method: 'get',
//     params: { token: getToken() }
//   })
// }

export function getSubOrgs(data) {
  return request({
    url: '/check/getSubOrgs',
    method: 'get',
    params: data
  })
}

export function logout() {
  return request({
    url: '/Account/Logout',
    method: 'get'
  })
}



import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/Account/load',
    method: 'get',
    params
  })
}
export function getUserList(params) {
  return request({
    url: '/Account/GetList',
    method: 'get',
    params
  })
}
export function get(params) {
  return request({
    url: '/users/get',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Account/CreateAccount',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Account/UpdateAccount',
    method: 'post',
    data
  })
}

export function changePassword(data) {
  return request({
    url: '/Account/ChangeUserPassword',
    method: 'post',
    data
  })
}
export function getUserProfile(params)
{
	return request({
	  url: '/Account/GetUser',
	  method: 'get',
	  params
	})
	
}
export function changeProfile(data) {
  return request({
    url: '/users/changeprofile',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/users/delete',
    method: 'post',
    data
  })
}

export function loadByRole(params) {
  return request({
    url: '/users/loadByRole',
    method: 'get',
    params
  })
}
export function LoadByOrg(params) {
  return request({
    url: '/users/LoadByOrg',
    method: 'get',
    params
  })
}
//导入用户文件
export function InportUser(data) {
  return request({
    url: '/Account/InportUser',

    method: 'post',
    data
  })
}
export function ExportUser(username, password) {
  return request({
    url: '/Account/ExportUser',
    method: 'Get',
    responseType: "blob"
  })
}

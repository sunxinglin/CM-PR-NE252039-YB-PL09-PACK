import request from '@/utils/request'

export function getListByType(params) {

  return request({
    url: '/categorys/getListByType',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/categorys/load',
    method: 'get',
    params
  })
}

export function loadForRole(roleId) {
  return request({
    url: '/categorys/loadForRole',
    method: 'get',
    params: { appId: '', firstId: roleId }
  })
}

export function add(data) {
  return request({
    url: '/categorys/add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/categorys/update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/categorys/delete',
    method: 'post',
    data
  })
}
export function loadType(params) {
  return request({
    url: '/CategoryTypes/Load',
    method: 'get',
    params
  })
}


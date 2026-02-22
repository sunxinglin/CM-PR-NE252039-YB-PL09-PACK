import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Pack/Load',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Pack/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Pack/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Pack/DeleteEntity',
    method: 'post',
    data
  })
}
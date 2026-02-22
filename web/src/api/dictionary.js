import request from '@/utils/request'

export function get(params) {

  return request({
    url: '/Dictionary/Get',
    method: 'get',
    params
  })
}

export function load(params) {
  return request({
    url: '/Dictionary/Load',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/Dictionary/GetList',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Dictionary/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Dictionary/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Dictionary/DeleteEntity',
    method: 'post',
    data
  })
}
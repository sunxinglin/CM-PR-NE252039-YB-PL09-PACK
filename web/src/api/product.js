import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Product/Load',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/Product/getList',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Product/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Product/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Product/DeleteEntity',
    method: 'post',
    data
  })
}

export function listForNoFlow(params) {
  return request({
    url: '/Product/GetlistForNoFlow',
    method: 'get',
    params
  })
}

export function loadListForNoFlow(params) {
  return request({
    url: '/Product/LoadListForNoFlow',
    method: 'get',
    params
  })
}
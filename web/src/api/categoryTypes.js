import request from '@/utils/request'

export function addType(data) {
    return request({
      url: '/CategoryType/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/CategoryType/Update',
      method: 'post',
      data
    })
  }
  
  export function delType(data) {
    return request({
      url: '/CategoryType/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function loadType(params) {
    return request({
      url: '/CategoryType/LordList',
      method: 'get',
      params
    })
  }
import request from '@/utils/request'

export function add(data) {
    return request({
      url: '/StationTaskAnyLoad/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTaskAnyLoad/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/StationTaskAnyLoad/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function load(params) {
    return request({
      url: '/StationTaskAnyLoad/LoadByTaskId',
      method: 'get',
      params
    })
  }
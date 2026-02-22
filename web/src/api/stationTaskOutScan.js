import request from '@/utils/request'

export function add(data) {
    return request({
      url: '/StationTaskUserScan/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTaskUserScan/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/StationTaskUserScan/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function load(params) {
    return request({
      url: '/StationTaskUserScan/LoadByTaskId',
      method: 'get',
      params
    })
  }
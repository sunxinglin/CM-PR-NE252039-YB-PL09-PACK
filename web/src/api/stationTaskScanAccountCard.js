import request from '@/utils/request'

export function add(data) {
    return request({
      url: '/StationTaskScanAccountCard/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTaskScanAccountCard/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/StationTaskScanAccountCard/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function load(params) {
    return request({
      url: '/StationTaskScanAccountCard/LoadByTaskId',
      method: 'get',
      params
    })
  }
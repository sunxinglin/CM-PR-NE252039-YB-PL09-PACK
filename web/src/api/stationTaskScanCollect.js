import request from '@/utils/request'

export function add(data) {
    return request({
      url: '/StationTaskScanCollect/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTaskScanCollect/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/StationTaskScanCollect/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function load(params) {
    return request({
      url: '/StationTaskScanCollect/LoadByTaskId',
      method: 'get',
      params
    })
  }
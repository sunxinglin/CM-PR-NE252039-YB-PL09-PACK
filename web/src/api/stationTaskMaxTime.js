import request from '@/utils/request'

export function add(data) {
    return request({
      url: '/StationTaskMaxTime/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTaskMaxTime/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/StationTaskMaxTime/DeleteEntity',
      method: 'post',
      data
    })
  }
  
  export function load(params) {
    return request({
      url: '/StationTaskMaxTime/LoadByTaskId',
      method: 'get',
      params
    })
  }
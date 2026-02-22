import request from '@/utils/request'
export function add(data) {
    return request({
      url: '/StationTaskScrew/Add',
      method: 'post',
      data
    })
  }

  export function update(data) {
    return request({
      url: '/StationTaskScrew/Update',
      method: 'post',
      data
    })
  }

    
  export function del(data) {
    return request({
      url:  '/StationTaskScrew/DeleteEntity',
      method: 'post',
      data
    })
  }

  export function Load(params) {
    return request({
      url: '/StationTaskScrew/LoadByTaskId',
      method: 'get',
      params
    })
  }

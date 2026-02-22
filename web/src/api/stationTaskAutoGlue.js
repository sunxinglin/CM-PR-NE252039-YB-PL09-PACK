import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskGlue/Update',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskGlue/Add',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskGlue/DeleteEntity',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/Base_StationTaskGlue/GetByTaskId',
      method: 'get',
      params
    })
  }

 
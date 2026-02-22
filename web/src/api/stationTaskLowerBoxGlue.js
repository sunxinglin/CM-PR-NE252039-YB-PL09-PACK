import request from '@/utils/request'

export function update(data) {
  return request({
    url: '/Base_StationTask_LowerBoxGlue/Update',
    method: 'post',
    data
  })
}
export function add(data) {
  return request({
    url: '/Base_StationTask_LowerBoxGlue/Add',
    method: 'post',
    data
  })
}
export function del(data) {
  return request({
    url: '/Base_StationTask_LowerBoxGlue/DeleteEntity',
    method: 'post',
    data
  })
}

export function GetByTaskId(params) {
  return request({
    url: '/Base_StationTask_LowerBoxGlue/GetByTaskId',
    method: 'get',
    params
  })
}


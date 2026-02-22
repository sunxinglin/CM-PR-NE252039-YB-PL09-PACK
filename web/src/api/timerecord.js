import request from '@/utils/request'

export function GetByPack(params) {
  return request({
    url: '/Proc_GluingTime/GetRecordTime',
    method: 'get',
    params
  })
}

export function UpdataTime(data) {
  return request({
    url: '/Proc_GluingTime/UpdateTime',
    method: 'post',
    data
  })
}
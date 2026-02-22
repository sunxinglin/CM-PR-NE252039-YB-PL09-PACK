import request from '@/utils/request'

export function GetByPack(params) {
  return request({
    url: '/Proc_GluingTime/GetByPack',
    method: 'get',
    params
  })
}

export function Updata(params) {
  return request({
    url: '/Proc_GluingTime/Updata',
    method: 'get',
    params
  })
}
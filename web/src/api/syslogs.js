import request from '@/utils/request'

export function getList(data) {
  return request({
    url: '/Syslog/GetList',
    method: 'post',
    data
  })
}





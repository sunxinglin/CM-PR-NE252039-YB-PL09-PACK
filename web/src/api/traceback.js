import request from '@/utils/request'
export function forwardgettask(params) {
  return request({
    url: '/ProductTraceBack/GetForward',
    method: 'get',
    params
  })
}
//导出Pack BOm文件
export function ModelExpornt(params) {
  return request({
    url: '/ProductTraceBack/ModelExpornt',
    method: 'get',
    params,
    responseType: "blob"
  })
}
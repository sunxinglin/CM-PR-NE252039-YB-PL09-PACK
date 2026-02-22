import request from '@/utils/request'

export function movehistory(params) {
  return request({
    url: '/HistoryData/MoveHistoryDataToRealTime_ByPackCode',
    method: 'get',
    params
  })
}

export function moverealtime(params) {
  return request({
    url: '/HistoryData/MoveRealTimeDataToHistory_ByPackCode',
    method: 'get',
    params
  })
}

export function clearhisdata_peifang(params) {
  return request({
    url: '/HistoryData/ClearHisData_PeiFang',
    method: 'get',
    params
  })
}

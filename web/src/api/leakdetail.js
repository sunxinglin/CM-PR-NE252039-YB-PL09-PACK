import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_StationTask_LeakDetail/Load',
        method: 'post',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/Proc_StationTask_LeakDetail/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}


import axios from 'axios'
import {
	Message,
	MessageBox
} from 'element-ui'
import store from '../store'
import user from '@/store/modules/user'
import {
	getToken
} from '@/utils/auth'

// 创建axios实例
const service = axios.create({
	baseURL: process.env.VUE_APP_BASE_API, // api的base_url
	timeout: 50000 // 请求超时时间
})

// request拦截器
service.interceptors.request.use(config => {
	
	config.headers['tenantId'] = store.getters.tenantid
	// if (user.newtime!=null && (parseInt(user.newtime -user.oldalivetime )/1000/60)>3 ) {
	// 	// MessageBox.confirm('登录已超时，可以【取消】继续留在该页面，或者【重新登录】', '超时提醒', {
	// 	// 	confirmButtonText: '重新登录',
	// 	// 	cancelButtonText: '取消',
	// 	// 	type: 'warning'
	// 	// }).then(() => {
	// 		store.dispatch('FedLogOut').then(() => {
	// 			location.reload() // 为了重新实例化vue-router对象 避免bug
	// 		})
	// 	// })
	// }
	// user.oldalivetime=user.newtime;
	// user.newtime=new Date();
	
		
		
	
	
	if (store.getters.token) {
		config.headers['X-Token'] = "123456" // 让每个请求携带自定义token 请根据实际情况自行修改
	}

	if (store.getters.isIdentityAuth) {
		config.headers['Authorization'] = 'Bearer ' + store.getters.oidcAccessToken
	}

	return config
}, error => {
	// Do something with request error
	console.log(error) // for debug
	Promise.reject(error)
})

// respone拦截器
service.interceptors.response.use(
	response => {
		console.log( response);
		
		const res = response.data

		if (res instanceof Blob) {
			if (res.type && res.type.indexOf("application/json") > -1) {
				let reader = new FileReader()
				reader.readAsText(res, 'utf-8')
				reader.onload = e => {
					let readerRes = reader.result
					let parseObj = {}
					parseObj = JSON.parse(readerRes)
					Message({
						message: parseObj.message,
						type: 'error',
						duration: 5 * 1000
					})
				}

				return Promise.reject('error');
			}
			return res
		}
		
		if (res.code == undefined) {
			if (res.type=="application/json") {
				let reader = new FileReader()
                    reader.readAsText(res, 'utf-8')
                    reader.onload = e => {
                        let readerRes = reader.result
                        let parseObj = {}
                        parseObj = JSON.parse(readerRes)
                        Message({
							message: parseObj.message,
							type: 'error',
							duration: 5 * 1000
						})
                    }

					return Promise.reject('error');
			}
			if (response.headers["content-type"]!=undefined && response.headers["content-type"]=="application/vnd.ms-excel") {
				return res
			}
			if (res.success) {

				return res.data
			} else {

				Message({
					message: res.message,
					type: 'error',
					duration: 5 * 1000
				})
				return Promise.reject('error')
			}
		} else {

			const res = response.data
			
			if (res.code !== 200) {
				
				// 50008:非法的token; 50012:其他客户端登录了;  50014:Token 过期了;
				if (res.code === 50008 || res.code === 50012 || res.code === 50014) {
					MessageBox.confirm('登录已超时，可以【取消】继续留在该页面，或者【重新登录】', '超时提醒', {
						confirmButtonText: '重新登录',
						cancelButtonText: '取消',
						type: 'warning'
					}).then(() => {
						store.dispatch('FedLogOut').then(() => {
							location.reload() // 为了重新实例化vue-router对象 避免bug
						})
					})
				} else {
					
					Message({
						message: res.message || res.msg,
						type: 'error',
						duration: 5 * 1000
					})
				}
			
				return Promise.reject('error')
			}
			else {
				return response.data
			}
		}
	},
	error => {
		const resp = error && error.response
		const data = resp && resp.data

		if (data instanceof Blob && data.type && data.type.indexOf('application/json') > -1) {
			let reader = new FileReader()
			reader.readAsText(data, 'utf-8')
			reader.onload = e => {
				let readerres = reader.result
				let parseObj = {}
				parseObj = JSON.parse(readerres)
				Message({
					message: parseObj.message,
					type: 'error',
					duration: 5 * 1000
				})
			}
			return Promise.reject(error)
		}

		if (data && (data.message || data.msg)) {
			Message({
				message: data.message || data.msg,
				type: 'error',
				duration: 5 * 1000
			})
			return Promise.reject(error)
		}

		Message({
			message: '请先启动接口站，再刷新本页面，异常详情：' + error.message,
			type: 'error',
			duration: 10 * 1000
		})
		return Promise.reject(error)
	}
)

export default service

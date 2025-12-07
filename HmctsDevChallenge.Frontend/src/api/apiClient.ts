import axios, {type AxiosError, type AxiosInstance} from 'axios';
import {ProblemDetailsError, ProblemDetailsSchema,} from "../errors/ProblemDetails.ts";
import {MalformedProblemDetailsError} from "../errors/MalformedProblemDetails.ts";

const ApiClient: AxiosInstance = axios.create({
	baseURL: '/api',
	headers: {
		'Content-Type': 'application/json',
	},
})

ApiClient.interceptors.response.use(
	response => response,
	(error: AxiosError) => {
		if (error.response?.headers['Content-Type'] !== 'application/problem+json') {
			return Promise.reject(error);
		}
		if (!error.response?.data) {
			return Promise.reject(new MalformedProblemDetailsError());
		}

		const problemDetails = ProblemDetailsSchema.safeParse(error.response.data);

		if (!problemDetails.success) {
			return Promise.reject(new MalformedProblemDetailsError(error.response.data));
		}

		return Promise.reject(new ProblemDetailsError(problemDetails.data));
	}
)

export default ApiClient;
import type {CreateTaskType, ReadTaskType} from "./types.ts";
import {ReadTaskSchema} from "./types.ts";
import ApiClient from "../apiClient.ts";
import {MalformedResponseError} from "../../errors/MalformedResponse.ts";

export const createTaskFn = async (task: CreateTaskType): Promise<ReadTaskType> => {
	const res = await ApiClient.post('/task', task);
	const body = ReadTaskSchema.safeParse(res.data);

	if (!body.success) {
		throw new MalformedResponseError(res.data as object, ReadTaskSchema, 'ReadTask');
	}

	return body.data;
}
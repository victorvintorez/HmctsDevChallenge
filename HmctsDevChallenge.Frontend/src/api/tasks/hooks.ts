import {useQueryClient} from "@tanstack/react-query";
import {createTaskFn} from "./functions.ts";
import {useProblemMutation} from "../useProblemMutation.ts";
import type {CreateTaskType, ReadTaskType} from "./types.ts";
import {useNavigate} from "@tanstack/react-router";


export const useCreateTaskHook = () => {
	const queryClient = useQueryClient();
	const navigate = useNavigate()

	return useProblemMutation<CreateTaskType, ReadTaskType>(
		createTaskFn,
		{
			onSuccess: async (data) => {
				queryClient.setQueryData<ReadTaskType>(['tasks', {id: data.id}], data)
				await navigate({to: '/task', search: data})
			}
		}
	)
}

export const useTaskAPI = {
	Create: useCreateTaskHook,
}
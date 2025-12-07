import {useMutation, type UseMutationOptions, type UseMutationResult} from "@tanstack/react-query";
import {ProblemDetailsError, type ProblemDetailsType} from "../errors/ProblemDetails.ts";


export interface UseProblemMutationResult<TData, TVariables> extends Omit<UseMutationResult<TVariables, Error, TData>, 'error'> {
	error: ProblemDetailsError | Error | null;
	problemDetails: ProblemDetailsType | null;
}

export function useProblemMutation<TVariables, TData>(
	mutationFn: (variables: TVariables) => Promise<TData>,
	options?: Omit<UseMutationOptions<TData, Error, TVariables>, 'mutationFn'>,
): UseProblemMutationResult<TVariables, TData> {
	const mutation = useMutation({
		mutationFn,
		...options,
	})

	const problemDetails = mutation.error instanceof ProblemDetailsError ? mutation.error.problemDetails : null;

	return {
		...mutation,
		error: mutation.error,
		problemDetails,
	};
}
import {MalformedResponseError} from "./MalformedResponse.ts";
import {ProblemDetailsSchema} from "./ProblemDetails.ts";

export class MalformedProblemDetailsError extends MalformedResponseError {
	constructor(
		data?: object
	) {
		super(data ?? {}, ProblemDetailsSchema, 'ProblemDetails');
		this.name = 'MalformedProblemDetailsError';
	}
}
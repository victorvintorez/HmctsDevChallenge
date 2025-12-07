import {z} from 'zod';

export const ProblemDetailsSchema = z.object({
	type: z.string().optional(),
	title: z.string().optional(),
	status: z.number().optional(),
	detail: z.string().optional(),
	instance: z.string().optional(),
	errors: z.record(z.string(), z.array(z.string())).optional()
})
export type ProblemDetailsType = z.infer<typeof ProblemDetailsSchema>;

export class ProblemDetailsError extends Error {
	public problemDetails: ProblemDetailsType;

	constructor(
		problemDetails: ProblemDetailsType,
	) {
		super(problemDetails.title ?? 'Problem Details Error: ' + problemDetails.status);
		this.name = 'ProblemDetailsError';
		this.problemDetails = problemDetails;
	}
}
import {createFileRoute} from '@tanstack/react-router'
import * as React from 'react'
import {type FormEvent, useCallback, useState} from 'react'
import {useTaskAPI} from "../api/tasks/hooks.ts";
import * as GovUK from "govuk-react";
import {CreateTaskSchema, type CreateTaskType} from "../api/tasks/types.ts";
import {useForm} from "@tanstack/react-form";


const IndexRouteComponent: React.FC = () => {
	const tomorrow = new Date();
	tomorrow.setDate(tomorrow.getDate() + 1);
	tomorrow.setHours(12, 0, 0, 0);

	const {problemDetails, isPending, mutateAsync} = useTaskAPI.Create();

	const [time, setTime] = useState<{ hours: number, minutes: number, ampm: 'am' | 'pm' }>({
		hours: 12,
		minutes: 0,
		ampm: 'pm'
	})
	const [date, setDate] = useState<{ day: number, month: number, year: number }>({
		day: tomorrow.getDate(),
		month: tomorrow.getMonth() + 1,
		year: tomorrow.getFullYear()
	})

	const form = useForm({
		defaultValues: {
			title: '',
			description: undefined,
			status: '',
			due: tomorrow.toISOString()
		} as CreateTaskType,
		validators: {
			onChange: CreateTaskSchema,
		},
		onSubmit: async ({value}) => await mutateAsync(value)
	})

	const handleFormSubmit = useCallback(async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		event.stopPropagation();
		if (isPending) return;
		await form.handleSubmit();
	}, [form, isPending]);

	const hasServerErrors = (): boolean => {
		return form.state.submissionAttempts > 0 &&
			(problemDetails?.errors !== undefined &&
				Object.keys(problemDetails?.errors).length > 0);
	}

	const getServerErrors = (): { targetName?: string, text?: string }[] => {
		const errorList: { targetName?: string, text?: string }[] = [];

		if (problemDetails?.errors) {
			Object.entries(problemDetails.errors).forEach(([key, messages]) => {
				messages.forEach((message) => {
					errorList.push({
						targetName: key,
						text: message,
					});
				});
			});
		}

		return errorList;
	}

	const getFieldServerErrors = (fieldName: keyof CreateTaskType): string[] | undefined => {
		if (problemDetails?.errors?.[fieldName]) {
			return problemDetails.errors[fieldName];
		}

		return undefined;
	}

	const toErrorList = (localErrors: string | string[] | undefined, serverErrors: string[] | undefined): string[] | undefined => {
		if (!localErrors && !serverErrors) return undefined;

		const errors: string[] = [];

		if (localErrors) {
			if (Array.isArray(localErrors)) {
				errors.push(...localErrors);
			} else {
				errors.push(localErrors);
			}
		}

		if (serverErrors) {
			errors.push(...serverErrors);
		}

		if (errors.length === 0) return undefined;

		return errors;
	}

	return (
		<>
			<form onSubmit={(ev) => void handleFormSubmit(ev)}>
				<GovUK.LoadingBox loading={isPending}>
					{hasServerErrors() && (
						<GovUK.ErrorSummary
							heading="Error Summary"
							description="Please correct the following errors"
							errors={getServerErrors()}
						/>
					)}
					<GovUK.Fieldset>
						<GovUK.Fieldset.Legend size="L">Add Task</GovUK.Fieldset.Legend>
						<form.Field name="title" children={({state, handleChange, handleBlur}) => (
							<GovUK.InputField
								mb={4}
								meta={{
									touched: state.meta.isTouched,
									error: toErrorList(state.meta.errors.flatMap(err => err?.message ?? []), getFieldServerErrors('title'))
								}}
								input={{onChange: e => handleChange(e.target.value), name: 'title'}}
								onBlur={handleBlur}
							>
								Title
							</GovUK.InputField>
						)}/>
						<form.Field name="description" children={({state, handleChange, handleBlur}) => (
							<GovUK.TextArea
								mb={4}
								meta={{
									touched: state.meta.isTouched,
									error: toErrorList(state.meta.errors.flatMap(err => err?.message ?? []), getFieldServerErrors('description'))
								}}
								input={{onChange: e => handleChange(e.target.value), name: 'description'}}
								onBlur={handleBlur}
							>
								Description
							</GovUK.TextArea>
						)}/>
						<form.Field name="status" children={({state, handleChange, handleBlur}) => (
							<GovUK.InputField
								mb={4}
								meta={{
									touched: state.meta.isTouched,
									error: toErrorList(state.meta.errors.flatMap(err => err?.message ?? []), getFieldServerErrors('status'))
								}}
								input={{
									onChange: e => handleChange(e.target.value),
									name: 'status',
									value: state.value
								}}
								onBlur={handleBlur}
							>
								Status
							</GovUK.InputField>
						)}/>
						<form.Field name="due" validators={{
							onChange: ({value}) => {
								const parsedDate = CreateTaskSchema.shape.due.safeParse(value)
								if (!parsedDate.success) {
									return parsedDate.error.issues.map((e) => e.message).join(', ');
								}
								return undefined;
							}
						}} children={({state, handleChange, handleBlur}) => {
							const updateDueDate = (newTime: typeof time, newDate: typeof date) => {
								if (
									isNaN(newTime.hours) ||
									isNaN(newTime.minutes) ||
									isNaN(newDate.day) ||
									isNaN(newDate.month) ||
									isNaN(newDate.year)
								) {
									handleChange('invalid-date');
									handleBlur();
									return;
								}

								if (newTime.hours < 1 || newTime.hours > 12
									|| newTime.minutes < 0 || newTime.minutes > 59
									|| newDate.day < 1 || newDate.day > 31
									|| newDate.month < 1 || newDate.month > 12
									|| newDate.year < 2000 || newDate.year > 2999) {
									handleChange('invalid-date');
									handleBlur();
									return;
								}

								const hours = newTime.ampm === 'pm' ? (newTime.hours % 12) + 12 : newTime.hours % 12;
								const dueDate = new Date(newDate.year, newDate.month - 1, newDate.day, hours, newTime.minutes);

								if (isNaN(dueDate.getTime())) {
									handleChange('invalid-date');
									handleBlur();
									return;
								}

								handleChange(dueDate.toISOString());
								handleBlur();
							}

							const errorList = toErrorList(state.meta.errors.flatMap(err => typeof (err) === "string" ? err : []), getFieldServerErrors('due'));
							const hasErrors = state.meta.isTouched && errorList && errorList.length > 0;

							return (
								<GovUK.Fieldset>
									<GovUK.Fieldset.Legend size="M">Due Date</GovUK.Fieldset.Legend>
									{hasErrors && (
										<GovUK.ErrorText>
											{errorList.map((error, idx) => (
												<div key={idx}>{error}</div>
											))}
										</GovUK.ErrorText>
									)}
									<GovUK.FormGroup style={{display: 'flex', gap: '1rem', marginBottom: '1rem'}}>
										<GovUK.InputField
											mb={2}
											style={{width: '75px'}}
											input={{
												name: 'due-hours',
												type: 'number',
												min: 1,
												max: 12,
												value: isNaN(time.hours) ? '' : time.hours,
												onChange: e => {
													const newHours = parseInt(e.target.value, 10);
													const newTime = {...time, hours: newHours};
													setTime(newTime);
													updateDueDate(newTime, date);
												}
											}}
											onBlur={handleBlur}
										>
											Hours
										</GovUK.InputField>
										<GovUK.InputField
											mb={2}
											style={{width: '75px'}}
											input={{
												name: 'due-minutes',
												type: 'number',
												min: 0,
												max: 59,
												value: isNaN(time.minutes) ? '' : time.minutes,
												onChange: e => {
													const newMinutes = parseInt(e.target.value, 10);
													const newTime = {...time, minutes: newMinutes};
													setTime(newTime);
													updateDueDate(newTime, date);
												}
											}}
											onBlur={handleBlur}
										>
											Minutes
										</GovUK.InputField>
										<GovUK.Select
											mb={2}
											label="AM/PM"
											style={{width: '150px'}}
											input={{
												name: 'due-ampm',
												value: time.ampm,
												onChange: e => {
													const newAmPm = e.target.value as 'am' | 'pm';
													const newTime = {...time, ampm: newAmPm};
													setTime(newTime);
													updateDueDate(newTime, date);
												}
											}}
											onBlur={handleBlur}
										>
											<option value="am">AM</option>
											<option value="pm">PM</option>
										</GovUK.Select>
									</GovUK.FormGroup>
									<GovUK.FormGroup style={{display: 'flex', gap: '1rem'}}>
										<GovUK.InputField
											mb={2}
											style={{width: '75px'}}
											input={{
												name: 'due-day',
												type: 'number',
												min: 1,
												max: 31,
												value: isNaN(date.day) ? '' : date.day,
												onChange: e => {
													const newDay = parseInt(e.target.value, 10);
													const newDate = {...date, day: newDay};
													setDate(newDate);
													updateDueDate(time, newDate);
												}
											}}
											onBlur={handleBlur}
										>
											Day
										</GovUK.InputField>
										<GovUK.InputField
											mb={2}
											style={{width: '75px'}}
											input={{
												name: 'due-month',
												type: 'number',
												min: 1,
												max: 12,
												value: isNaN(date.month) ? '' : date.month,
												onChange: e => {
													const newMonth = parseInt(e.target.value, 10);
													const newDate = {...date, month: newMonth};
													setDate(newDate);
													updateDueDate(time, newDate);
												}
											}}
											onBlur={handleBlur}
										>
											Month
										</GovUK.InputField>
										<GovUK.InputField
											mb={2}
											style={{width: '75px'}}
											input={{
												name: 'due-year',
												type: 'number',
												min: 2000,
												max: 2999,
												value: isNaN(date.year) ? '' : date.year,
												onChange: e => {
													const newYear = parseInt(e.target.value, 10);
													const newDate = {...date, year: newYear};
													setDate(newDate);
													updateDueDate(time, newDate);
												}
											}}
											onBlur={handleBlur}
										>
											Year
										</GovUK.InputField>
									</GovUK.FormGroup>
								</GovUK.Fieldset>
							)
						}}/>
						<GovUK.Button type="submit" disabled={form.state.isSubmitting}>Create Task</GovUK.Button>
					</GovUK.Fieldset>
				</GovUK.LoadingBox>
			</form>
		</>
	)
}

export const Route = createFileRoute('/')({
	component: IndexRouteComponent,
})


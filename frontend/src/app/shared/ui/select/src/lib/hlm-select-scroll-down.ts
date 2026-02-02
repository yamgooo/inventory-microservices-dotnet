import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideChevronDown } from '@ng-icons/lucide';
import { classes } from '@spartan-ng/helm/utils';

@Component({
	selector: 'hlm-select-scroll-down',
	imports: [NgIcon],
	providers: [provideIcons({ lucideChevronDown })],
	changeDetection: ChangeDetectionStrategy.OnPush,
	template: `
		<ng-icon class="ml-2 size-4" name="lucideChevronDown" />
	`,
})
export class HlmSelectScrollDown {
	constructor() {
		classes(() => 'flex cursor-default items-center justify-center py-1');
	}
}

import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeftRight, lucideFilter, lucidePlus } from '@ng-icons/lucide';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmTableImports } from '@spartan-ng/helm/table';
import { HlmDropdownMenuImports } from '@spartan-ng/helm/dropdown-menu';
import { HlmItemImports } from '@spartan-ng/helm/item';
import { HlmSpinnerImports } from '@spartan-ng/helm/spinner';
import { HlmSheetImports } from '@spartan-ng/helm/sheet';
import { BrnSheetImports } from '@spartan-ng/brain/sheet';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { TransactionService } from '@core/services/transaction.service';
import { Transaction, TransactionFilters } from '@core/services/interfaces/transaction.model';
import { TransactionFormComponent } from './transaction-form.component';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [
    CommonModule,
    NgIcon,
    HlmInputImports,
    HlmButtonImports,
    HlmTableImports,
    HlmDropdownMenuImports,
    HlmItemImports,
    HlmSpinnerImports,
    HlmSheetImports,
    BrnSheetImports,
    FormsModule,
    TransactionFormComponent
  ],
  templateUrl: './transactions.html',
  providers: [provideIcons({ lucideArrowLeftRight, lucideFilter, lucidePlus })]
})
export class Transactions implements OnInit, OnDestroy {
  isLoading = signal(false);
  transactions = signal<Transaction[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  pageSize = 10;

  filters = {
    transactionType: undefined as number | undefined,
    startDate: undefined as string | undefined,
    endDate: undefined as string | undefined,
    minAmount: undefined as number | undefined,
    maxAmount: undefined as number | undefined
  };

  selectedId = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(private transactionService: TransactionService) {}

  loadTransactions(): void {
    this.isLoading.set(true);

    const requestFilters: TransactionFilters = {
      transactionType: this.filters.transactionType,
      startDate: this.filters.startDate,
      endDate: this.filters.endDate,
      minAmount: this.filters.minAmount,
      maxAmount: this.filters.maxAmount,
      page: this.currentPage(),
      pageSize: this.pageSize
    };

    const sub = this.transactionService
      .getTransactions(requestFilters)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: response => {
          if (response?.success && response?.data) {
            this.transactions.set(response.data.items);
            this.totalCount.set(response.data.totalCount);
            this.totalPages.set(response.data.totalPages);
            this.currentPage.set(response.data.page);
          } else {
            this.transactions.set([]);
          }
        },
        error: error => {
          console.error(error);
          this.transactions.set([]);
        }
      });

    this.subscriptions.add(sub);
  }

  ngOnInit(): void {
    this.loadTransactions();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  applyFilters(): void {
    this.currentPage.set(1);
    this.loadTransactions();
  }

  clearAllFilters(): void {
    this.filters = {
      transactionType: undefined,
      startDate: undefined,
      endDate: undefined,
      minAmount: undefined,
      maxAmount: undefined
    };
    this.currentPage.set(1);
    this.loadTransactions();
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(current => current - 1);
      this.loadTransactions();
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(current => current + 1);
      this.loadTransactions();
    }
  }

  openCreateForm(): void {
    this.selectedId.set(null);
  }

  selectTransaction(id: string, sheet: any): void {
    this.selectedId.set(id);
    sheet.open();
  }

  handleSaved(): void {
    this.loadTransactions();
  }
}
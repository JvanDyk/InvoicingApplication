namespace AndreyevInterview.Shared.Services.Interfaces;

public interface IInvoicesService
{
    /// <summary>
    /// Retrieves all invoices.
    /// </summary>
    /// <returns>Returns the InvoiceModel which holds a list of invoices.</returns>
    Task<InvoiceModel> GetInvoicesAsync();

    /// <summary>
    /// Retrieves the history of a specific invoice.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <returns>Returns the InvoiceHistoryDTO which holds the history of the specified invoice.</returns>
    Task<InvoiceHistory> GetInvoiceHistoryAsync(int id);

    /// <summary>
    /// Retrieves line items for a specific invoice.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <returns>The LineItemModel which holds line items for the specified invoice.</returns>
    LineItemModel GetInvoiceLineItems(int id);

    /// <summary>
    /// Creates a new invoice.
    /// </summary>
    /// <param name="input">The invoice input model.</param>
    /// <returns>Returns the created InvoiceEntity.</returns>
    Task<InvoiceEntity> CreateInvoiceAsync(InvoiceInput input);

    /// <summary>
    /// Deletes a specific invoice.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    Task DeleteInvoiceAsync(int id);

    /// <summary>
    /// Adds a discount to a specific invoice.
    /// </summary>
    /// <param name="invoiceId">The ID of the invoice.</param>
    /// <param name="discount">The discount percentage to be added.</param>
    /// <returns>Returns the updated InvoiceEntity.</returns>
    Task<InvoiceEntity> AddDiscountAsync(int invoiceId, int discount);

    /// <summary>
    /// Adds a line item to a specific invoice.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <param name="input">The line item input model.</param>
    /// <returns>Returns the added LineItemEntity.</returns>
    Task<LineItemEntity> AddLineItemEntityToInvoiceAsync(int id, LineItemInput input);

    /// <summary>
    /// Updates the billable status for a line item.
    /// </summary>
    /// <param name="lineItemEntity">The line item billable model.</param>
    /// <returns>The result indicates whether the billable status was successfully updated.</returns>
    Task<bool> UpdateBillableAsync(LineItemBillable lineItemEntity);

    /// <summary>
    /// Deletes a specific line item.
    /// </summary>
    /// <param name="id">The ID of the line item.</param>
    /// <returns>The result indicates whether the line item was successfully deleted.</returns>
    Task<bool> DeleteLineItemEntityAsync(int id);
}
